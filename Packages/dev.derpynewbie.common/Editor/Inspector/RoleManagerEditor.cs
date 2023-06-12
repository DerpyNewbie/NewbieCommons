using System;
using System.Collections.Generic;
using DerpyNewbie.Common.Role;
using UdonSharpEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DerpyNewbie.Common.Editor.Inspector
{
    [CustomEditor(typeof(RoleManager))]
    public class RoleManagerEditor : UnityEditor.Editor
    {
        private List<KeyValuePair<string, MessageType>> _issues = new List<KeyValuePair<string, MessageType>>();

        private ReorderableList _rolesList;
        private List<int> _rolesExpanded = new List<int>();
        private ReorderableList _playersList;
        private List<int> _playersExpanded = new List<int>();

        private void OnEnable()
        {
            FindIssues();
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();


            var roles = serializedObject.FindProperty("availableRoles");
            var players = serializedObject.FindProperty("players");

            if (DrawIssues())
            {
                var defaultRoleData = roles.GetArrayElementAtIndex(0).objectReferenceValue as RoleData;
                EditorGUILayout.HelpBox(
                    $"Default role will be '{(defaultRoleData != null ? defaultRoleData.RoleName : null)}'\n{players.arraySize} players will be processed with assigned roles.",
                    MessageType.Info);
            }

            if (_rolesList == null || _playersList == null)
            {
                _rolesExpanded = new List<int>();
                _rolesList = new ReorderableList(serializedObject, roles)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Roles"),
                    elementHeightCallback = index =>
                    {
                        if (!_rolesExpanded.Contains(index))
                            return EditorGUIUtility.singleLineHeight * 4;

                        var element = roles.GetArrayElementAtIndex(index);
                        if (element.objectReferenceValue == null)
                            return EditorGUIUtility.singleLineHeight * 4;

                        var serializedElementObj = new SerializedObject(element.objectReferenceValue);

                        return EditorGUIUtility.singleLineHeight *
                               (5 + serializedElementObj.FindProperty("roleProperties").arraySize);
                    },
                    drawElementCallback = (rect, index, active, focused) =>
                    {
                        rect.height = EditorGUIUtility.singleLineHeight;
                        rect.y += rect.height / 2;
                        var element = roles.GetArrayElementAtIndex(index);

                        EditorGUI.PropertyField(rect, element,
                            new GUIContent(index == 0 ? "Default Role" : $"Element {index}"));

                        if (element.objectReferenceValue == null)
                            return;

                        var serialized = new SerializedObject(element.objectReferenceValue);
                        var roleName = serialized.FindProperty("roleName");
                        var roleProperties = serialized.FindProperty("roleProperties");

                        rect.y += rect.height;
                        EditorGUI.PropertyField(rect, roleName, new GUIContent("Name"));

                        rect.y += rect.height;
                        switch (EditorGUI.Foldout(rect, _rolesExpanded.Contains(index), "Properties"))
                        {
                            case true when !_rolesExpanded.Contains(index):
                                _rolesExpanded.Add(index);
                                break;
                            case false when _rolesExpanded.Contains(index):
                                _rolesExpanded.Remove(index);
                                break;
                            case true:
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    rect.y += rect.height;
                                    roleProperties.arraySize =
                                        EditorGUI.DelayedIntField(rect, "Size", roleProperties.arraySize);
                                    for (var i = 0; i < roleProperties.arraySize; i++)
                                    {
                                        rect.y += rect.height;
                                        EditorGUI.PropertyField(rect, roleProperties.GetArrayElementAtIndex(i));
                                    }
                                }

                                break;
                        }

                        if (serialized.ApplyModifiedProperties())
                        {
                            FindIssues();

                            var actualInstance = ((RoleData)element.objectReferenceValue);
                            if (actualInstance.gameObject.name != actualInstance.RoleName)
                                actualInstance.gameObject.name = actualInstance.RoleName;
                        }
                    },
                    onAddDropdownCallback = (rect, list) =>
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add and assign generated RoleData"), false, () =>
                        {
                            // Generate new RoleData GameObject
                            var go = new GameObject($"GeneratedRole-{list.count}");
                            var roleData = go.AddUdonSharpComponent<RoleData>();
                            var serializedRoleData = new SerializedObject(roleData);

                            serializedRoleData.FindProperty("roleName").stringValue = go.name;
                            serializedRoleData.ApplyModifiedProperties();

                            // Set parent to RoleManager/Roles
                            var roleManagerTransform = ((RoleManager)target).transform;
                            var rolesParent = roleManagerTransform.Find("Roles");
                            if (rolesParent == null)
                            {
                                Debug.Log($"[RoleManagerEditor] Creating Roles parent", roleManagerTransform);
                                rolesParent = new GameObject("Roles").transform;
                                rolesParent.SetParent(roleManagerTransform);
                            }

                            go.transform.SetParent(rolesParent);

                            // Add generated RoleData to roles array
                            ++roles.arraySize;
                            var index = roles.arraySize - 1;
                            var element = roles.GetArrayElementAtIndex(index);

                            element.objectReferenceValue = roleData;
                            element.serializedObject.ApplyModifiedProperties();

                            // Auto-expand element
                            _rolesExpanded.Add(index);

                            FindIssues();
                        });

                        menu.AddItem(new GUIContent("Add empty element"), false, () =>
                        {
                            ++roles.arraySize;
                            roles.GetArrayElementAtIndex(roles.arraySize - 1).objectReferenceValue = null;
                            roles.serializedObject.ApplyModifiedProperties();

                            FindIssues();
                        });

                        menu.DropDown(rect);
                    },
                    onRemoveCallback = (list) =>
                    {
                        var obj = roles.GetArrayElementAtIndex(list.index).objectReferenceValue;
                        if (obj != null)
                        {
                            DestroyImmediate((obj as Component)?.gameObject);
                        }

                        var lastArraySize = roles.arraySize;
                        roles.DeleteArrayElementAtIndex(list.index);
                        if (lastArraySize == roles.arraySize)
                            roles.DeleteArrayElementAtIndex(list.index);

                        FindIssues();
                    }
                };

                _playersExpanded = new List<int>();
                _playersList = new ReorderableList(serializedObject, players)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Players"),
                    elementHeightCallback = index =>
                    {
                        if (!_playersExpanded.Contains(index))
                            return EditorGUIUtility.singleLineHeight * 4;

                        var element = players.GetArrayElementAtIndex(index);
                        if (element.objectReferenceValue == null)
                            return EditorGUIUtility.singleLineHeight * 4;

                        var serializedElement = new SerializedObject(element.objectReferenceValue);

                        return EditorGUIUtility.singleLineHeight *
                               (5 + serializedElement.FindProperty("roles").arraySize);
                    },
                    drawElementCallback = (rect, index, active, focused) =>
                    {
                        rect.y += EditorGUIUtility.singleLineHeight / 2;
                        rect.height = EditorGUIUtility.singleLineHeight;
                        var element = players.GetArrayElementAtIndex(index);
                        EditorGUI.PropertyField(rect, element);

                        if (element.objectReferenceValue == null)
                            return;

                        var serialized = new SerializedObject(element.objectReferenceValue);
                        var displayName = serialized.FindProperty("displayName");
                        var playerRoles = serialized.FindProperty("roles");

                        rect.y += rect.height;
                        EditorGUI.PropertyField(rect, displayName);

                        rect.y += rect.height;
                        switch (EditorGUI.Foldout(rect, _playersExpanded.Contains(index), "Roles"))
                        {
                            case true when !_playersExpanded.Contains(index):
                                _playersExpanded.Add(index);
                                break;
                            case false when _playersExpanded.Contains(index):
                                _playersExpanded.Remove(index);
                                break;
                            case true:
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    rect.y += rect.height;
                                    playerRoles.arraySize =
                                        EditorGUI.DelayedIntField(rect, "Size", playerRoles.arraySize);
                                    for (var i = 0; i < playerRoles.arraySize; i++)
                                    {
                                        rect.y += rect.height;
                                        EditorGUI.PropertyField(rect, playerRoles.GetArrayElementAtIndex(i));
                                    }
                                }

                                break;
                        }

                        if (serialized.ApplyModifiedProperties())
                        {
                            FindIssues();

                            var actualInstance = ((RolePlayerData)element.objectReferenceValue);
                            if (actualInstance.gameObject.name != actualInstance.DisplayName)
                                actualInstance.gameObject.name = actualInstance.DisplayName;
                        }
                    },
                    onAddDropdownCallback = (rect, list) =>
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add and assign generated RolePlayerData"), false, () =>
                        {
                            // Generate new RolePlayerData GameObject
                            var go = new GameObject($"GeneratedPlayer-{list.count}");
                            var playerData = go.AddUdonSharpComponent<RolePlayerData>();
                            var serializedPlayerData = new SerializedObject(playerData);

                            serializedPlayerData.FindProperty("displayName").stringValue = go.name;
                            serializedPlayerData.ApplyModifiedProperties();

                            // Set parent to RoleManager/Players
                            var roleManagerTransform = ((RoleManager)target).transform;
                            var playersParent = roleManagerTransform.Find("Players");
                            if (playersParent == null)
                            {
                                Debug.Log($"[RoleManagerEditor] Creating Players parent", roleManagerTransform);
                                playersParent = new GameObject("Players").transform;
                                playersParent.SetParent(roleManagerTransform);
                            }

                            go.transform.SetParent(playersParent);

                            // Add generated RolePlayerData to roles array
                            ++players.arraySize;
                            var index = players.arraySize - 1;
                            var element = players.GetArrayElementAtIndex(index);

                            element.objectReferenceValue = playerData;
                            element.serializedObject.ApplyModifiedProperties();

                            // Auto-expand element
                            _playersExpanded.Add(index);

                            FindIssues();
                        });

                        menu.AddItem(new GUIContent("Add empty element"), false, () =>
                        {
                            ++players.arraySize;
                            players.GetArrayElementAtIndex(players.arraySize - 1).objectReferenceValue = null;
                            players.serializedObject.ApplyModifiedProperties();

                            FindIssues();
                        });

                        menu.DropDown(rect);
                    },
                    onRemoveCallback = (list) =>
                    {
                        var obj = players.GetArrayElementAtIndex(list.index).objectReferenceValue;
                        if (obj != null)
                        {
                            DestroyImmediate((obj as Component)?.gameObject);
                        }

                        var lastArraySize = players.arraySize;
                        players.DeleteArrayElementAtIndex(list.index);
                        if (lastArraySize == players.arraySize)
                            players.DeleteArrayElementAtIndex(list.index);

                        FindIssues();
                    }
                };
            }

            _rolesList.DoLayoutList();
            EditorGUILayout.Space();
            _playersList.DoLayoutList();

            if (serializedObject.ApplyModifiedProperties())
                FindIssues();
        }

        private bool DrawIssues()
        {
            if (_issues.Count == 0)
                return true;

            foreach (var kvp in _issues)
            {
                EditorGUILayout.HelpBox(kvp.Key, kvp.Value);
            }

            return false;
        }

        private void FindIssues()
        {
            _issues = new List<KeyValuePair<string, MessageType>>();

            var roles = serializedObject.FindProperty("availableRoles");
            if (roles.arraySize == 0)
            {
                _issues.Add(new KeyValuePair<string, MessageType>(
                    "No available roles assigned will cause crash at runtime.\nPlease add default role!",
                    MessageType.Error
                ));
            }

            if (HasEmptyRolePlayer())
            {
                _issues.Add(new KeyValuePair<string, MessageType>(
                    "RolePlayerData with no role assigned will cause crash at runtime.\nPlease assign valid roles!",
                    MessageType.Error
                ));
            }

            if (HasNullInProperties())
            {
                _issues.Add(new KeyValuePair<string, MessageType>(
                    "Having 'None' in setting will cause issues at runtime.\nPlease assign valid object!",
                    MessageType.Error
                ));
            }

            if (HasDuplicatedDisplayName())
            {
                _issues.Add(new KeyValuePair<string, MessageType>(
                    "There is RolePlayerData with same display name assigned.\nThis may result in confusing behaviour!",
                    MessageType.Warning
                ));
            }

            if (HasDuplicatedRoleName())
            {
                _issues.Add(new KeyValuePair<string, MessageType>(
                    "There is RoleData with same name assigned.\nThis may result in confusing behaviour!",
                    MessageType.Warning
                ));
            }
        }

        private bool HasDuplicatedDisplayName()
        {
            var players = serializedObject.FindProperty("players");
            var seenDisplayNames = new List<string>();
            for (int i = 0; i < players.arraySize; i++)
            {
                var element = players.GetArrayElementAtIndex(i);
                var objRef = element.objectReferenceValue;
                if (objRef == null)
                    continue;

                var playerData = objRef as RolePlayerData;
                if (playerData == null)
                    continue;

                if (seenDisplayNames.Contains(playerData.DisplayName))
                    return true;

                seenDisplayNames.Add(playerData.DisplayName);
            }

            return false;
        }

        private bool HasDuplicatedRoleName()
        {
            var roles = serializedObject.FindProperty("availableRoles");
            var seenRoleNames = new List<string>();
            for (int i = 0; i < roles.arraySize; i++)
            {
                var element = roles.GetArrayElementAtIndex(i);
                var objRef = element.objectReferenceValue;
                if (objRef == null)
                    continue;

                var roleData = objRef as RoleData;
                if (roleData == null)
                    continue;

                if (seenRoleNames.Contains(roleData.RoleName))
                    return true;

                seenRoleNames.Add(roleData.RoleName);
            }

            return false;
        }

        private bool HasEmptyRolePlayer()
        {
            var players = serializedObject.FindProperty("players");
            for (int i = 0; i < players.arraySize; i++)
            {
                var element = players.GetArrayElementAtIndex(i);
                var objRef = element.objectReferenceValue;
                if (objRef == null)
                    continue;

                var playerData = objRef as RolePlayerData;
                if (playerData == null)
                    continue;

                if (playerData.Roles.Length == 0)
                    return true;

                if (playerData.Roles[0] == null)
                    return true;
            }

            return false;
        }

        private bool HasNullInProperties()
        {
            var roles = serializedObject.FindProperty("availableRoles");
            var players = serializedObject.FindProperty("players");

            return HasNullInArrayProperty(roles) || HasNullInArrayProperty(players);
        }

        private bool HasNullInArrayProperty(SerializedProperty arrayProperty)
        {
            if (!arrayProperty.isArray) throw new ArgumentException("Non-array property was provided");
            for (var i = 0; i < arrayProperty.arraySize; i++)
            {
                var element = arrayProperty.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == null)
                    return true;
            }

            return false;
        }
    }
}
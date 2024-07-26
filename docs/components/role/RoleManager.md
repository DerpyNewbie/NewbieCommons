# RoleManager

RoleManager はすべての [RolePlayerData](./RolePlayerData) や [RoleData](./RoleData) をまとめ、用意された API を通して楽にロールのデータを取得できます。

## Properties

| Property Name   | Description                                                                                                   |
|-----------------|---------------------------------------------------------------------------------------------------------------|
| Players         | すべての RolePlayerData を保持することで、 API を通して名前や VRCPlayerApi を通して取得することが可能になります。                                    |
| Available Roles | すべての RoleData を保持することで、API を通して取得することが可能になります。<br/>また、ここに指定されている最初のロールがデフォルトのロールとして、指定がないすべてのプレイヤーに対して適用されます。 |

## Methods

| Method Name                      | Description                                      |
|----------------------------------|--------------------------------------------------|
| GetPlayerNamesOf(RoleData, bool) | 指定した RoleData を持っているプレイヤーの Display Name を返します。   |
| GetPlayersOf(RoleData)           | 指定した RoleData を持っているインスタンス内の VRCPlayerApi を返します。 |
| GetPlayerRoles(VRCPlayerApi)     | 指定した VRCPlayerApi が持っている RoleData[] を返します        |
var __defProp = Object.defineProperty;
var __name = (target, value) => __defProp(target, "name", {value, configurable: true});

// worker.ts
var worker_default = {
    async fetch(request, env, ctx) {
        async function MethodNotAllowed(request2) {
            return new Response(`Method ${request2.method} not allowed.`, {
                status: 405,
                headers: {
                    Allow: "GET"
                }
            });
        }

        __name(MethodNotAllowed, "MethodNotAllowed");
        if (request.method !== "GET") return MethodNotAllowed(request);
        const url = new URL(request.url);

        if (!url.pathname.toLowerCase().startsWith("/newbiecommons")) {
            console.log("looking at garbage");
            return new Response("Looks like Not Found", {
                status: 404
            });
        }

        url.pathname = url.pathname.toLowerCase().replace(/^\/newbiecommons/, "") || "/";

        return env.ASSETS.fetch(new Request(url, request));
    }
};
export {
    worker_default as default
};
//# sourceMappingURL=worker.js.map
import { AzureFunction, Context, HttpRequest } from "@azure/functions";
import { loadNuxt } from "nuxt";

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest
): Promise<void> {
  context.log("HTTP trigger function processed a request.");
  let nuxt = await loadNuxt("start");
  let result = await nuxt.renderRoute(new URL(req.url).pathname, context);

  let { html, cspScriptSrcHashes, error, redirected, preloadFiles } = result;

  console.log(error, redirected);
  context.res = {
    body: html,
    headers: {
      ["Content-Type"]: "text/html; charset=utf-8",
      ["Accept-Ranges"]: "none",
      ["Content-Length"]: Buffer.byteLength(html),
    },
  };
};

export default httpTrigger;

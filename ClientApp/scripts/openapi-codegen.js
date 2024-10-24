import { generate } from "openapi-typescript-codegen";
import { resolve } from "path";

process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = "0"

const schema = await fetch("https://localhost:7525/swagger/v1/swagger.json").then(res => res.json())

await generate({
    input: schema,
    httpClient: "fetch",
    useUnionTypes: true,
    output: resolve("src", "openapi"),
    request: resolve("scripts", "request.ts"),
})
# Identity Server

In the following description, Identity Server will be referred to as `idsrv`.

## Use Cases

1. **`idsrv` Provides OIDC Login Method**  
To begin, `idsrv` will provide external provider authentication like google.

1. **`idsrv` Provides JWT Authentication to Protect APIs**

1. **Traefik Forwards Authentication to the `idsrv`**  
`idsrv` is used to authenticate requests received by Traefik. `idsrv` responds with HTTP `302 Redirect` to authentication requests. The user is prompted to enter credentials or use external identity provider (e.g. google) to login. Upon successful login the user is redirected to the original request with appropriate authentication values.

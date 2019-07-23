# Identity Server

In the following description, Identity Server will be referred to as `idsrv`.

## Use Cases

1. **`idsrv` Provides OIDC Login Method**  
To begin, `idsrv` will provide external provider authentication like google.

1. **`idsrv` Provides JWT Authentication to Protect APIs**

1. **Traefik Forwards Authentication to the `idsrv`**  
`idsrv` is used to authenticate requests received by Traefik. `idsrv` responds with HTTP `302 Redirect` to authentication requests. The user is prompted to enter credentials or use external identity provider (e.g. google) to login. Upon successful login the user is redirected to the original request with appropriate authentication values.

## Identity Server Extensions

1. Identity Server Options: User Interaction  
Set the various login/logout urls

1. IProfileService  
https://identityserver4.readthedocs.io/en/latest/reference/profileservice.html

1. IResourceStore

1. IClientStore

1. ICorsPolicyService

1. Entity Framework Support  
https://identityserver4.readthedocs.io/en/latest/reference/ef.html

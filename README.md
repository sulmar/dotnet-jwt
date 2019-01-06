# dotnet-jwt
Przykład autentykacji z użyciem JWT w .NET 4.5

## Basic Authentication

![Basic Authentication](https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/basic-authentication/_static/image1.png)

*	Klient wysyła żądanie do serwera

``` GET http://localhost:62986/api/values HTTP/1.1 ```

*	Zamiast zasobu serwer odpowiada komunikatem z kodem 401
``` 
WWW-Authenticate: Basic realm="localhost"
Content-Length=500
Content-Type= application/json
{”message”: ”jakiś komunikat”}
``` 

*	Klient ponownie wysyła żądanie o zasób, ale dodaje nagłówek "Authorization" z wartością zbudowaną na podstawie loginu i hasła w formacie: ``` base64(login:password) ```

```
GET http://localhost:62986/api/values HTTP/1.1 
Authorization: Basic am9objpqamo= 
```

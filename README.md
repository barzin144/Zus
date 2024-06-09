# ![Zus](https://raw.githubusercontent.com/barzin144/zus/main/zus.png) _Zus CLI Tool_

[![NuGet Package: zus](https://img.shields.io/nuget/v/zus?logo=nuget&label=NuGet&color=4169E1)](https://www.nuget.org/packages/zus)
[![Build](https://github.com/barzin144/Zus/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/barzin144/Zus/actions/workflows/ci.yml)

## Installation
```Shell
dotnet tool install --global zus
```
## Send Request
#### Get
```Shell
zus send get https://jsonplaceholder.typicode.com/posts/1
```
| Options     | Description                                   | example                                                                   |
| ----------- | --------------------------------------------- | ------------------------------------------------------------------------- |
| -a, --auth  | Authentication Bearer Token                   | zus send get https://jsonplaceholder.typicode.com/posts/1 -a eyJhbGciOi== |
| -n, --name  | Name for saving the request                   | zus send get https://jsonplaceholder.typicode.com/posts/1 -n posts        |
| -f, --force | Overwrite the existing request with same name | zus send get https://jsonplaceholder.typicode.com/posts/1 -n posts -f     |
#### Post
```Shell
zus send post http://localhost:5000/api/Account/Login -d '{"username":"zus","password":"123456"}'
```
| Options     | Description                                   |
| ----------- | --------------------------------------------- |
| -d, --data  | Json data, wrap your data in single quote.    |
| -a, --auth  | Authentication Bearer Token                   |
| -n, --name  | Name for saving the request                   |
| -f, --force | Overwrite the existing request with same name |
#### Resend
>  Send a saved request.
```Shell
zus resend login
```
## Base64
#### Encode
```Shell
zus base64 my_text
```
> output: bXlfdGV4dA==
#### Decode
```Shell
zus dbase64 bXlfdGV4dA==
```
> output: my_text
## SHA256
```Shell
zus sha256 my_text
```
> output: wK5CFkMfgStqjLvxe/h7zaqzNISGyy2xWP9dN893UEI=
## Decode JWT
![image](https://raw.githubusercontent.com/barzin144/zus/main/.github/assets/zus-djwt.png)
#### With Secret
```Shell
zus djwt eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Ilp1cyBDTEkgVG9vbCIsImlhdCI6MTUxNjIzOTAyMn0.9BmMva7XRwYtaNkvmobWNNQX8lHyGnSyVRuzgCjEcIY my_secret
```
```Json
{
  "alg": "HS256",
  "typ": "JWT"
}
{
  "sub": "1234567890",
  "name": "Zus CLI Tool",
  "iat": 1516239022
}
Signature Verified
```
#### Without Secret
```Shell
zus djwt eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Ilp1cyBDTEkgVG9vbCIsImlhdCI6MTUxNjIzOTAyMn0.9BmMva7XRwYtaNkvmobWNNQX8lHyGnSyVRuzgCjEcIY
```
```Json
{
  "alg": "HS256",
  "typ": "JWT"
}
{
  "sub": "1234567890",
  "name": "Zus CLI Tool",
  "iat": 1516239022
}
Invalid Signature
```


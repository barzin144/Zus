# ![Zus](https://raw.githubusercontent.com/barzin144/zus/main/zus.png) _Zus CLI Tool_

[![NuGet Package: zus](https://img.shields.io/nuget/v/zus?logo=nuget&label=NuGet&color=4169E1)](https://www.nuget.org/packages/zus)
[![Mutation](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/barzin144/9f0cbbbaf1e2c85744909ca282bdf21c/raw/stryker.json)](https://barzin144.github.io/Zus/reports/index.html)
[![Build](https://github.com/barzin144/Zus/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/barzin144/Zus/actions/workflows/ci.yml)
[![Test](https://github.com/barzin144/Zus/actions/workflows/test.yml/badge.svg?branch=main)](https://github.com/barzin144/Zus/actions/workflows/test.yml)

# Table of Contents  
- [Installation](#installation)
- [Send Request](#send-request)
    - [Send Get Request](#get)
    - [Send Post Request](#post)
    - [Send A Request With Pre-Request](#pre-request)
    - [Resend A Request](#resend)
- [Variable](#variable)
- [Request](#request)
- [Base64 Decoder/Encoder](#base64)
- [Sha256 Hash](#sha256)
- [Guid](#guid)
- [JWT Decoder](#jwt-decoder)
## Installation
```Shell
dotnet tool install --global zus
```
## Send Request
#### Get
```Shell
zus send get https://jsonplaceholder.typicode.com/posts/1
```
| Options            | Description                                   | example                                                                   |
| ------------------ | --------------------------------------------- | ------------------------------------------------------------------------- |
| -a, --auth         | Authentication Bearer Token                   | zus send get https://jsonplaceholder.typicode.com/posts/1 -a eyJhbGciOi== |
| -n, --name         | Name for saving the request                   | zus send get https://jsonplaceholder.typicode.com/posts/1 -n posts        |
| -p, --pre-request  | Pre-request name                              | zus send get https://jsonplaceholder.typicode.com/posts/1 -p login        |
| -f, --force        | Overwrite the existing request with same name | zus send get https://jsonplaceholder.typicode.com/posts/1 -n posts -f     |
#### Post
```Shell
zus send post http://localhost:5000/api/Account/LoginWithJsonData -d "username:zus,password:123456"
zus send post http://localhost:5000/api/Account/LoginWithFormData -x -d "username:zus,password:123456"
```
| Options            | Description                                                                                                                                                           |
| ------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| -d, --data         |  Data format: `Key:Value,Key:Value` and wrap your data in single or double quote. Data will be sent in Json format by default. By adding -x flag change format to form-urlencoded |
| -a, --auth         | Authentication Bearer Token                                                                                                                                           |
| -n, --name         | Name for saving the request                                                                                                                                           |
| -x, --form-format  | Convert Key:Value data to form-urlencoded format                                                                                                                      |
| -j, --json-format  | Convert Key:Value data to json format                                                                                                                                 |
| -p, --pre-request  | Pre-request name                                                                                                                                                      |
| -f, --force        | Overwrite the existing request with same name                                                                                                                         |
#### Pre-Request
```Shell
zus send post http://localhost:5000/api/Account/Login -n login -d "username:zus,password:123456"
//response: { "accessToken": "eyJhbGciOiJI..." }

zus send post http://localhost:5000/api/Account/UpdateProfile -p login -a "{pr.accessToken}" -d "name:zus-tool"

zus send get http://localhost:5000/api/Product/1 -n product
//response: { "id": "ABC123", name: "PC" }
zus send post http://localhost:5000/api/Product/Update -p product -d "product_id:{pr.id},name:laptop"

zus send post http://localhost:5000/api/Account/GetToken -n get-token -d "username:zus,password:123456"
//response: "eyJhbGciOiJI..."

zus send post http://localhost:5000/api/Account/UpdateProfile -p get-token -a "{pr.$}" -d "name:zus-tool"

zus send post http://localhost:5000/api/Account/UpdateProfile -p get-token -d "name:{var.name}"

zus send post http://localhost:5000/api/Account/Login -x -d "username:zus,password:123456" //form-urlencoded format
zus send post http://localhost:5000/api/Account/Login -j -d "username:zus,password:123456" //json format
zus send post http://localhost:5000/api/Account/Login -d '{"username": "zus", "password": "123456"}' //string format

```
> *{pr.KEY_OF_RESPONSE_OBJECT}* will be replaced with Pre-request response data.(If Pre-request response data is Json)

> *{pr.$}* will be replaced with Pre-request response data.(If Pre-request response data is String)

> *{var.VARIABLE_NAME}* will be replaced with value of saved variable.
#### Resend
>  Send a saved request.
```Shell
zus resend login
```
## Variable
>  Manage variables.
#### List
```Shell
zus var list
```
#### Delete
```Shell
zus var delete SAVED_NAME
```
## Request
>  Access to saved requests.
#### List
```Shell
zus request list
```
#### Delete
```Shell
zus request delete SAVED_NAME
```
## Base64
#### Encode
```Shell
zus base64 my_text
zus base64 -f FILE_PATH
```
> output: bXlfdGV4dA==
#### Decode
```Shell
zus dbase64 bXlfdGV4dA==
zus dbase64 -f bXlfdGV4dA== //store decoded data into txt file
```
> output: my_text
## SHA256
```Shell
zus sha256 my_text
```
> output: wK5CFkMfgStqjLvxe/h7zaqzNISGyy2xWP9dN893UEI=
## GUID
```Shell
zus guid
```
> output: c4a3bb74-bc13-4cfc-8487-8eefe7912c54
## JWT Decoder
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


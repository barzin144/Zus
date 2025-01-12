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
- [Epoch Time Converter](#epoch-time-converter)
- [JWT Decoder](#jwt-decoder)

## Installation

```Shell
dotnet tool install --global zus
```

## Send Request

#### Get

```Shell
zus send get https://example.com/posts/1
```

| Options             | Description                                                                                      | example                                                   |
| ------------------- | ------------------------------------------------------------------------------------------------ | --------------------------------------------------------- |
| -a, --auth          | Authentication Bearer Token                                                                      | zus send get https://example.com/posts/1 -a eyJhbGciOi==  |
| -n, --name          | Name for saving the request                                                                      | zus send get https://example.com/posts/1 -n posts         |
| -s, --save-response | Save response                                                                                    | zus send get https://example.com/posts/1 -s               |
| -h, --header        | Add header to request, format: Key:Value,Key:Value and wrap your data in single or double quote. | zus send get https://example.com/posts/1 -h 'api-key:abc' |
| -p, --pre-request   | Pre-request name                                                                                 | zus send get https://example.com/posts/1 -p login         |
| -f, --force         | Overwrite the existing request with same name                                                    | zus send get https://example.com/posts/1 -n posts -f      |

#### Post

```Shell
zus send post http://example/api/Account/Login -x -d "username:zus,password:123456" //form-urlencoded format
zus send post http://example/api/Account/Login -j -d "username:zus,password:123456" //json format
zus send post http://example/api/Account/Login -d '{"username": "zus", "password": "123456"}' //string format
```

| Options             | Description                                                                                                                                                                                              |
| ------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| -d, --data          | Data format: `Key:Value,Key:Value` and wrap your data in single or double quote. Data will be sent in string format by default. By adding -x flag change format to form-urlencoded or -j for Json format |
| -a, --auth          | Authentication Bearer Token                                                                                                                                                                              |
| -n, --name          | Name for saving the request                                                                                                                                                                              |
| -s, --save-response | Save response                                                                                                                                                                                            |
| -h, --header        | Add header to request, format: Key:Value,Key:Value and wrap your data in single quote or double quote.                                                                                                   |
| -x, --form-format   | Convert Key:Value data to form-urlencoded format                                                                                                                                                         |
| -j, --json-format   | Convert Key:Value data to json format                                                                                                                                                                    |
| -p, --pre-request   | Pre-request name                                                                                                                                                                                         |
| -f, --force         | Overwrite the existing request with same name                                                                                                                                                            |

#### Pre-Request

```Shell
zus send post http://example/api/Account/Login -n login -d "username:zus,password:123456"
//response: { "accessToken": "eyJhbGciOiJI..." }

zus send post http://example/api/Account/UpdateProfile -p login -a "{pr.accessToken}" -j -d "name:zus-tool"

zus send get http://example/api/Product/1 -n product
//response: { "id": "ABC123", name: "PC" }
zus send post http://example/api/Product/Update -p product -j -d "product_id:{pr.id},name:laptop"

zus send post http://example/api/Account/GetToken -n get-token -j -d "username:zus,password:123456"
//response: "eyJhbGciOiJI..."

zus send post http://example/api/Account/UpdateProfile -p get-token -a "{pr.$}" -j -d "name:zus-tool"

zus send post http://example/api/Account/UpdateProfile -p get-token -d "name:{var.name}"

```

> _{pr.KEY_OF_RESPONSE_OBJECT}_ will be replaced with Pre-request response data.(If Pre-request response data is Json)

> _{pr.$}_ will be replaced with Pre-request response data.(If Pre-request response data is String)

> _{var.VARIABLE_NAME}_ will be replaced with value of saved variable.

#### Resend

> Send a saved request.

```Shell
zus resend login
```

## Variable

> Manage variables.

#### List

```Shell
zus var list
```

#### Delete

```Shell
zus var delete SAVED_NAME
```

## Request

> Access to saved requests.

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

## Epoch Time Converter

```Shell
zus dt 1734895248
```

> output: UTC: Sunday, December 22, 2024 7:20:48 PM +00:00
> Local: Monday, December 23, 2024 3:20:48 AM +08:00

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

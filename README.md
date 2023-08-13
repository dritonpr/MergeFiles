# API Documentation

## Overview
 This is a .NET 7.0 Web API project that provides functionality to merge multiple .ZIP archive files into a single .ZIP file.

## Getting Started
To get a local copy up and running, follow these simple steps:
## Prerequisites
This application is built with .NET 7.0. Make sure you have the .NET 7.0 SDK installed on your machine.

### Installation
Clone the repo: git clone https://github.com/dritonpr/MergeFiles

### Project Architecture
#### Interface-Based Implementation
 API adheres to best practices by using an interface-based implementation. This approach decouples the implementation from the interface, promoting clean architecture, scalability, maintainability, and ease of testing.

### Endpoints
The API exposes the following endpoints:
### 1. POST /api/MergeArchives
### 2. POST /api/MergeArchives/mergezipfileandsaveondisc

Both endpoint merges multiple .ZIP files into one .ZIP file. 
#### Further explanation:
1. POST /api/MergeArchives, is implemented by using MemoryStream. It would be usefull for merging small sized files and when server processing memory it's not an issue.
2. POST /api/MergeArchives/mergezipfileandsaveondisc. This endpint uses disk-based operations and avoids loading the entire ZIP content into memory. The trade-off is the disk I/O, but for large ZIP files, it is more scalable than memory-based solutions. This approach avoid high memory usage, slow performance, and potential application crashes.
   - Temporarily save uploaded ZIP files to disk.
   - Stream directly from these saved files during processing.
   - Delete temporary files once done.
   - Return merged zip file.

#### Parameters:
- `files`: A collection of .ZIP files, submitted as Form-Data. This method supports the submission of multiple .ZIP files, each of which can be up to 100 MB (3 GB optionally, it's managed by appsettings.json variable).
I've chosen to use multipart/form-data for file uploads. This is a common choice for file uploads in web applications. Its support is widespread, making it easy to interact with from various clients (like browsers, curl, or Postman).
#### Responses:
- `200 OK`: If the operation is successful. The response body will contain the merged .ZIP file. 
- `400 Bad Request`: If the request is malformed or if any input .ZIP file is corrupted.
- `500 Internal Server Error`: In case of an unhandled exception. The response body will contain the error message "This should not have happened. We are already working on a solution.".

## API Usage
### Input
The API endpoint `/api/MergeArchives` expects a POST request containing multiple .ZIP files, sent as `multipart/form-data`. This format is standard for sending files over HTTP and is supported by many HTTP clients and libraries. The decision to use `multipart/form-data` is because it allows sending large files efficiently without the need to base64 encode them.

### Output
The API will respond with a .ZIP file containing the merged contents of the input files.

### Error Handling
If an unexpected error occurs during the process, the API will not expose detailed .NET exception messages to the client. Instead, it will respond with a generic error message and a `500 Internal Server Error` status code.

## Quality Requirements
1. **Scalability**: We implemented async patterns to ensure the server can handle multiple requests concurrently without blocking. This ensures the server remains responsive even under heavy load.
2. **Robustness**: By limiting the maximum file size, we ensure that the server doesn't get overwhelmed with too large files. We also catch unexpected exceptions and prevent their details from being sent to clients, thereby hiding potentially sensitive information.
3. **Efficiency**: The merging process is optimized to ensure it happens as quickly as possible, minimizing the response time for the client by streaming the data when possible to avoid excessive memory consumption.

## Implementation Details
- **Runtime**: The application is developed using ASP.NET Core, which is a cross-platform, high-performance framework for building modern, cloud-based, and internet-connected applications.
- **Error Handling**: To ensure unexpected errors are caught and appropriately handled, we use global exception handling middleware in ASP.NET Core. This prevents detailed .NET error messages from being sent to clients.

## Libraries and Dependencies
- **AspNetCoreRateLimit**: ASP.NET Core rate limiting solution designed to control the rate of requests that clients can make to a Web API or MVC app based on IP address or client ID.
- **Serilog.AspNetCore**: This package routes ASP.NET Core log messages through Serilog, so you can get information about ASP.NET's internal operations written to the same Serilog sinks as your application events.
- **Microsoft.AspNetCore.OpenApi**: Provides APIs for annotating route handler endpoints in ASP.NET Core with OpenAPI annotations.
- **Swashbuckle.AspNetCore**: Swagger tools for documenting APIs built on ASP.NET Core.

## API Features and Middleware

### Logging with Serilog
Our API uses Serilog to log detailed and structured events, which assists in debugging, error tracking, and auditing client usage.

#### Configuration and Usage:
- **Initialization**: Serilog is initialized in the `Program.cs` with sinks such as Console and File to redirect logs accordingly.
- **Log Level**: By default, the API logs Warning level and above events.
- **Data**: HTTP requests, along with response status and any internal exceptions, are logged and will be stored in daily create text files. Detailed .NET exceptions, however, are not exposed in the API responses for security purposes.

### Rate Limiting
Our API uses `AspNetCoreRateLimit` to prevent abuse by limiting the number of permissible requests a client can make within a specified timeframe.

#### Configuration and Usage:
- **Limit by IP**: Each IP is restricted based on the defined limits in the `appsettings.json`. 
- **Headers**: When a client is approaching their limit, the API will return headers (`X-Rate-Limit-Limit`, `X-Rate-Limit-Remaining`) indicating the total limit and the remaining number of requests allowed in the current window.
- **Limit Exceeded**: Once a client exceeds their rate limit, they will receive an HTTP 429 (Too Many Requests) status code with a retry-after header indicating when they can make the next request.
- **Rules**: The default rule, as documented, limits each IP to 1 requests per 2 second. This can be customized based on the use case and expected traffic.

## Testing
You can test the API using tools like Postman, cURL, or any HTTP client that supports `multipart/form-data` requests.

## Future planinng for improvement:
- Scan the uploaded files for potential malware, especially if they will be accessible to other users or if they'll be processed further by your application.
- Adding unit testing.
## Conculsion
Handling large files is challenging, but with careful consideration of memory usage, efficient processing, and the right infrastructure, it's feasible. I would also prefer to monitor the system's performance and make adjustments as needed.

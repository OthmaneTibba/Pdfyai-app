# PDFYAI

## Description of the application

PDFy AI is an application designed to revolutionize how users interact with their PDF documents.
ALl you will need to do is to upload your pdf document then you can start chat with it.

## Tech Stack

- Angualr
- Dotnet core web api
- Amzon S3
- OpenAi
- Paypal
- Google auth

## How chat is created ?

```mermaid
sequenceDiagram
    participant User
    participant WebApp
    participant S3
    participant PineconeDB

    User->>WebApp: Open chat screen for a specific PDF
    WebApp->>S3: Fetch PDF from S3 bucket
    S3-->>WebApp: PDF file
    WebApp->>WebApp: Read PDF data as string
    WebApp->>WebApp: Split string into small chunks
    loop For each chunk
        WebApp->>PineconeDB: Save chunk as embedded vector
    end
    PineconeDB-->>WebApp: Chunks saved
    WebApp-->>User: Chat created
```

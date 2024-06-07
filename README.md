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

## How we chat with the dociment ?

```mermaid
sequenceDiagram
    participant User
    participant WebApp
    participant OpenAI
    participant PineconeDB
    participant GPT3
    participant AngularApp

    User->>WebApp: Enter question
    WebApp->>OpenAI: Convert question to embedded vector
    OpenAI-->>WebApp: Result
    WebApp->>PineconeDB: Query similarity vectors
    PineconeDB-->>WebApp: Plain text result
    WebApp->>WebApp: Put result in context of prompt
    WebApp->>GPT3: Send prompt
    GPT3-->>WebApp: Response (stream)
    loop For each stream
        WebApp->>AngularApp: Send REST request
    end
```

## some picture of the application

![](./1.png)
![](./3.png)

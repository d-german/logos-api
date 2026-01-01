# OpenAI GPT Actions - Complete Guide

**Date:** January 1, 2026  
**Source:** OpenAI Official Documentation & OpenAI Cookbook  
**Purpose:** Custom API integration for ChatGPT GPTs

---

## Overview

GPT Actions allow Custom GPTs to connect to external APIs, enabling them to access real-world data and perform operations beyond ChatGPT's built-in capabilities. Actions are defined using the OpenAPI specification standard.

---

## What is a GPT Action?

An **Action** is a custom API integration that allows a GPT to:
- Connect to external databases
- Integrate with third-party services
- Perform real-world operations (e.g., booking, payments, data retrieval)
- Access private or proprietary data
- Execute custom business logic

---

## GPT Action Flow

1. **Create GPT** in ChatGPT UI (manually or using GPT builder)
2. **Identify APIs** you want to integrate
3. **Configure Action** in "Configure" tab → "Create new action"
   - Select authentication schema
   - Input OpenAPI specification
   - Set privacy policy URL
4. **Set Visibility**: "Only me", "Anyone with a link", or "Everyone"
5. **Users Interact** with your GPT
   - GPT injects action info into model context
   - Model determines when to call API based on user request
   - Returns API response to user

---

## Authentication Methods

### 1. None (No Authentication)

**Use Case:** Public APIs, initial "signed out" experiences

**Pros:**
- No user friction
- Immediate access
- Good for discovery/trial

**Cons:**
- No personalization
- Limited access control
- No usage tracking per user

**Example:** Public weather API, dictionary lookup

---

### 2. API Key Authentication

**Use Case:** Service-level authentication without individual user logins

**Configuration:**
- Add API key in GPT editor
- Key is encrypted in storage
- Sent with every request

**Pros:**
- Simple setup
- Service-level access control
- Request tracking
- Rate limiting capabilities

**Cons:**
- No per-user personalization
- Shared rate limits
- All users share same permissions

**Best For:** Internal tools, protected but not user-specific data

**Example:** LogosAPI (biblical text lookup - same data for all users)

---

### 3. OAuth

**Use Case:** Per-user authentication and personalized experiences

**Configuration:**
```yaml
Authentication: OAuth
Client ID: your_client_id
Client Secret: your_client_secret
Authorization URL: https://your-domain.com/oauth/authorize
Token URL: https://your-domain.com/oauth/token
Scope: read write
```

**OAuth Flow:**
1. User triggers action requiring auth
2. ChatGPT shows "Sign in to [domain]" button
3. User redirects to authorization URL
4. User approves permissions
5. ChatGPT receives access token
6. Token included in subsequent requests: `Authorization: Bearer <token>`

**OAuth Request Format:**
```json
{
  "grant_type": "authorization_code",
  "client_id": "YOUR_CLIENT_ID",
  "client_secret": "YOUR_CLIENT_SECRET",
  "code": "abc123",
  "redirect_uri": "https://chatgpt.com/aip/g-some_gpt_id/oauth/callback"
}
```

**Expected Response:**
```json
{
  "access_token": "example_token",
  "token_type": "bearer",
  "refresh_token": "example_token",
  "expires_in": 59
}
```

**Security Requirements:**
- Must use state parameter (CSRF protection)
- Client credentials follow OAuth best practices
- Refresh token support recommended

**Best For:** User-specific data, personalized actions, account operations

---

## OpenAPI Schema Structure

Actions use **OpenAPI 3.0 or 3.1** specification format.

### Minimum Required Schema

```yaml
openapi: 3.1.0
info:
  title: API Title
  description: Clear description of what this API does
  version: 1.0.0
servers:
  - url: https://your-api.com
    description: Production server
paths:
  /endpoint:
    get:
      operationId: uniqueOperationId
      summary: Brief summary of what this operation does
      description: Detailed description for the model
      parameters:
        - name: param1
          in: path
          required: true
          schema:
            type: string
          description: What this parameter represents
      responses:
        '200':
          description: Success response
          content:
            application/json:
              schema:
                type: object
                properties:
                  result:
                    type: string
```

### Key Components

#### info Section
- **title**: API name (shown to users)
- **description**: What the API does (helps model understand)
- **version**: API version

#### servers Section
- **url**: Base URL for API endpoints
- **description**: Server purpose (dev, staging, prod)

#### paths Section
Defines all available endpoints and operations.

#### operationId
- **Required**: Unique identifier for each operation
- **Purpose**: Model uses this to decide which operation to call
- **Format**: camelCase recommended (e.g., `getUserProfile`, `searchVerses`)

#### summary vs description
- **summary**: Short one-liner (model decision-making)
- **description**: Detailed explanation (model understanding)

#### parameters
- **in**: `path`, `query`, `header`, `cookie`
- **required**: boolean
- **schema**: Data type definition
- **description**: Clear explanation (critical for model)

#### requestBody
For POST/PUT/PATCH operations:
```yaml
requestBody:
  required: true
  content:
    application/json:
      schema:
        type: object
        properties:
          field1:
            type: string
            description: What this field represents
        required:
          - field1
```

#### responses
Define expected responses with examples:
```yaml
responses:
  '200':
    description: Successful operation
    content:
      application/json:
        schema:
          type: object
        example:
          message: "Success"
  '404':
    description: Resource not found
```

---

## Complete Example: LogosAPI Schema

```yaml
openapi: 3.1.0
info:
  title: LogosAPI - Biblical Text Analysis
  description: Retrieve Bible verses with original Greek/Hebrew text and lexicon definitions
  version: 1.0.0
servers:
  - url: https://your-app.koyeb.app
    description: Production server
paths:
  /api/verses/lookup:
    post:
      operationId: lookupVerses
      summary: Look up Bible verses with lexicon data
      description: |
        Retrieves verse text with original language tokens and Strong's lexicon definitions.
        Accepts verse references in format: Matt.1.1, John.3.16, Rom.3.23
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                verseReferences:
                  type: array
                  items:
                    type: string
                  description: Array of verse references (e.g., ["Matt.1.1", "John.3.16"])
                  example: ["Matt.1.1", "John.3.16"]
              required:
                - verseReferences
      responses:
        '200':
          description: Successfully retrieved verses with lexicon data
          content:
            application/json:
              schema:
                type: object
                properties:
                  verses:
                    type: array
                    items:
                      type: object
                      properties:
                        reference:
                          type: string
                        tokens:
                          type: array
                          items:
                            type: object
                        lexiconEntries:
                          type: object
        '400':
          description: Invalid verse reference format
        '404':
          description: Verse not found
  /api/lexicon/lookup:
    post:
      operationId: lookupLexicon
      summary: Look up Strong's lexicon definitions
      description: Retrieves lexicon entries for given Strong's numbers
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                strongsNumbers:
                  type: array
                  items:
                    type: string
                  example: ["G976", "G1078"]
      responses:
        '200':
          description: Successfully retrieved lexicon entries
          content:
            application/json:
              schema:
                type: object
  /health:
    get:
      operationId: healthCheck
      summary: Check API health status
      responses:
        '200':
          description: API is healthy
          content:
            text/plain:
              schema:
                type: string
                example: "Healthy"
```

---

## Consequential Flag

Mark operations that modify data or have significant consequences.

### Usage

```yaml
paths:
  /booking:
    post:
      operationId: bookHotel
      description: Books a hotel room and charges the user
      x-openai-isConsequential: true
```

### Behavior

| Value | User Experience | Use Case |
|-------|----------------|----------|
| `true` | Always prompts for confirmation, no "always allow" | Payments, bookings, deletions |
| `false` | Shows "always allow" button | Read operations, searches |
| Not set | GET=false, others=true | Default behavior |

### Examples

**Consequential (true):**
- Making payments
- Booking/reservations
- Deleting data
- Sending emails
- Placing orders

**Not Consequential (false):**
- Reading data
- Searching
- Fetching information
- Health checks

---

## Limitations & Constraints

### Technical Limits

| Limit | Value |
|-------|-------|
| Request payload | 100,000 characters max |
| Response payload | 100,000 characters max |
| Timeout | 45 seconds |
| Content types | Text only (no images/video) |
| Custom headers | Not supported |

### OAuth Domain Restrictions

- All OAuth domains must match primary endpoint domain
- **Exceptions:** Google, Microsoft, Adobe (can use different domains)

### Example Constraint Handling

If returning large Bible chapters, paginate or chunk data:
```json
{
  "verses": [ /* limited subset */ ],
  "hasMore": true,
  "nextPage": 2
}
```

---

## Production Requirements

### 1. TLS/HTTPS Required

- **Must use TLS 1.2 or later**
- Port 443 with valid public certificate
- No self-signed certificates

### 2. IP Allowlisting

ChatGPT calls from these CIDR blocks:
```
23.102.140.112/28
13.66.11.96/28
104.210.133.240/28
20.97.188.144/28
20.161.76.48/28
52.234.32.208/28
52.156.132.32/28
40.84.180.128/28
```

**Recommendation:** Allowlist these IPs in your firewall

### 3. Rate Limiting

- **Implement rate limiting** on your API
- ChatGPT respects `429 Too Many Requests` responses
- Backs off after receiving multiple 429s or 500s
- Returns graceful error messages to user

**Example ASP.NET Core Rate Limiting:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 60;
    });
});
```

### 4. Error Handling

Return meaningful HTTP status codes:
- `200` - Success
- `400` - Bad request (invalid parameters)
- `401` - Unauthorized
- `404` - Resource not found
- `429` - Too many requests
- `500` - Internal server error

---

## Best Practices

### Schema Design

1. **Clear Descriptions**
   - Write descriptions for the AI, not humans
   - Explain what data is needed and why
   - Include examples in schema

2. **Meaningful Operation IDs**
   - Use descriptive camelCase
   - Examples: `searchBibleVerses`, `getUserBookmarks`, `createAnnotation`

3. **Comprehensive Examples**
   - Include example requests and responses
   - Show edge cases in descriptions

4. **Type Safety**
   - Define precise schemas
   - Use enums where applicable
   - Mark required fields

### API Design

1. **RESTful Conventions**
   - GET for reading
   - POST for creating/complex queries
   - PUT/PATCH for updates
   - DELETE for removal

2. **Consistent Responses**
   - Standard error format
   - Always return JSON (unless text/plain appropriate)
   - Include helpful error messages

3. **Performance**
   - Keep responses under 100K characters
   - Return within 45 seconds
   - Consider pagination for large datasets

### Security

1. **Authentication**
   - Start with "None" for MVP/testing
   - Use API Key for service-level protection
   - Implement OAuth for user-specific data

2. **Validation**
   - Validate all inputs
   - Sanitize parameters
   - Return 400 for invalid requests

3. **Privacy**
   - Provide privacy policy URL
   - Be transparent about data usage
   - Follow GPT policies

---

## Testing Your Action

### 1. Test in GPT Editor

- Use the "Test" feature in GPT editor
- Check if parameters are extracted correctly
- Verify response format

### 2. Manual API Testing

```bash
# Test your API directly first
curl -X POST https://your-api.com/api/verses/lookup \
  -H "Content-Type: application/json" \
  -d '{"verseReferences": ["Matt.1.1"]}'
```

### 3. Validate OpenAPI Schema

Use tools like:
- Swagger Editor: https://editor.swagger.io/
- OpenAPI Validator
- Postman

### 4. Test OAuth Flow

- Ensure redirect URI matches
- Verify token exchange
- Test token refresh

---

## Common Pitfalls

### 1. Missing Descriptions

❌ **Bad:**
```yaml
operationId: lookup
summary: Lookup
```

✅ **Good:**
```yaml
operationId: lookupBibleVerses
summary: Look up Bible verses by reference
description: |
  Retrieves verse text with original Greek/Hebrew words and lexicon definitions.
  Accepts standard verse references like "Matt.1.1" or "John 3:16".
```

### 2. Unclear Parameters

❌ **Bad:**
```yaml
parameters:
  - name: ref
    in: query
    schema:
      type: string
```

✅ **Good:**
```yaml
parameters:
  - name: verseReference
    in: query
    required: true
    schema:
      type: string
      pattern: '^[A-Za-z0-9]+\.\d+\.\d+$'
      example: "Matt.1.1"
    description: Bible verse reference in format Book.Chapter.Verse
```

### 3. Missing Error Responses

Always define error responses:
```yaml
responses:
  '400':
    description: Invalid verse reference format
  '404':
    description: Verse not found in database
  '500':
    description: Internal server error
```

### 4. Timeout Issues

- Keep operations under 45 seconds
- Return partial results if needed
- Implement async patterns for long operations

---

## Privacy Policy Requirements

When sharing GPT publicly, must provide:
- **Privacy Policy URL** in GPT settings
- Clear statement of data usage
- How API stores/processes user data
- Data retention policies

**Example Statement:**
> "This GPT uses LogosAPI to retrieve biblical texts. No user data is stored. All requests are anonymous and not logged."

---

## Comparison: GPTs vs Assistants

| Feature | GPTs (with Actions) | Assistants API |
|---------|-------------------|----------------|
| Hosting | OpenAI hosts | You host |
| UI | ChatGPT UI | Custom UI required |
| Actions | Yes (OpenAPI) | Yes (Function Calling) |
| File handling | Limited | Full support |
| Code Interpreter | Yes | Yes |
| Cost | Free (with limits) | Pay per token |
| Sharing | GPT Store | Custom distribution |

---

## Example Use Cases

### 1. LogosAPI (Biblical Study)
- **Auth:** None or API Key
- **Endpoints:** Verse lookup, lexicon definitions
- **Users:** Theological researchers, students

### 2. E-commerce GPT
- **Auth:** OAuth
- **Endpoints:** Product search, add to cart, checkout
- **Consequential:** true for checkout

### 3. CRM Integration
- **Auth:** OAuth
- **Endpoints:** Contact lookup, create tasks, log activities
- **Consequential:** true for create/delete operations

### 4. Analytics Dashboard
- **Auth:** API Key
- **Endpoints:** Get metrics, generate reports
- **Consequential:** false (read-only)

---

## Quick Start Checklist

- [ ] Create or identify existing OpenAPI specification
- [ ] Choose authentication method
- [ ] Ensure API uses HTTPS with valid certificate
- [ ] Implement rate limiting
- [ ] Test API endpoints with curl/Postman
- [ ] Validate OpenAPI schema in Swagger Editor
- [ ] Create Custom GPT in ChatGPT
- [ ] Paste OpenAPI schema in Actions panel
- [ ] Configure authentication
- [ ] Add privacy policy URL
- [ ] Test GPT with various prompts
- [ ] Monitor API logs for errors
- [ ] Adjust descriptions based on model behavior
- [ ] Set appropriate consequential flags
- [ ] Deploy to intended audience

---

## Resources

- **OpenAI Actions Docs:** https://platform.openai.com/docs/actions
- **OpenAPI Specification:** https://swagger.io/specification/
- **OpenAI Cookbook Examples:** https://github.com/openai/openai-cookbook/tree/main/examples/chatgpt/gpt_actions_library
- **Community Forum:** https://community.openai.com/
- **Swagger Editor:** https://editor.swagger.io/

---

## Related Memories

- `application-architecture.md` - LogosAPI design
- `koyeb-deployment-guide.md` - API deployment
- `project-overview.md` - Project structure

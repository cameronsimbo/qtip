# QTip Coding Challenge

## Table of Contents

- [Expected Outputs](#expected-outputs)
- [Overview](#overview)
- [Requirements](#requirements)
  - [1. Frontend](#1-frontend)
  - [2. Backend](#2-backend)
  - [3. Database](#3-database)
- [Optional Extension: Multiple Classification Types](#optional-extension-multiple-classification-types)
- [Evaluation Criteria](#evaluation-criteria)
- [Deployment](#deployment)
- [Submission](#submission)

---

## Expected Outputs

By the end of this challenge, you should deliver:

### 1. Working Application
A fully functional system accessible via:
```bash
docker compose up
```

### 2. User Experience
- A web interface with a multiline text input
- Real-time visual detection of email addresses (underlined with tooltip)
- A submit button that sends text to the backend
- A statistics panel showing the total count of PII emails detected across all submissions

### 3. Backend Processing
- Detection and classification of email addresses with the tag `"pii.email"`
- Tokenization of detected emails (replacing them with unique tokens)
- Persistence of both tokenized text and classification records
- API endpoint(s) for retrieving statistics

### 4. Data Architecture
- Database schema supporting tokenized submissions and classification vault
- Clear separation between tokenized content and sensitive values

### 5. Documentation
A README explaining:
- How to run the application
- Architectural decisions made
- Any assumptions or trade-offs
- (Optional) Implementation notes for extensions

---

## Overview

Users frequently paste sensitive information into applications without realizing it. Your task is to build a prototype system called **QTip** that detects, classifies, and tokenizes sensitive data—functioning as a "data spellchecker" for personally identifiable information (PII).

The core concept is **data protection through tokenization**: when sensitive information is detected, it should be replaced with tokens, allowing the system to maintain context and analytics while handling the actual sensitive values securely.

### Objectives

This challenge evaluates your ability to:
- Design and implement end-to-end solutions
- Write clean, maintainable code
- Make pragmatic architectural decisions
- Balance trade-offs appropriately

### Scope

This is intentionally a small, focused challenge. You should be able to complete it in a few hours. You are free to choose your own architecture, folder structure, and implementation patterns.

### Need Help?

**Please don't struggle in silence.** If you encounter blockers, have questions about requirements, or need clarification on any aspect of the challenge, reach out to us. We value communication and the ability to ask for help when needed—these are important professional qualities.

---

## Requirements

### 1. Frontend

Build a web interface with the following components:

#### Text Input Area
- Multiline text input that accepts user typing and pasting
- Real-time detection of email addresses
- Visual indication: underline detected emails (squiggly or wavy style)
- Tooltip on hover: Display **"PII – Email Address"**
- Implementation approach is your choice (contenteditable, overlays, etc.)

#### Submit Button
- Triggers submission of the complete text content to the backend

#### Statistics Panel
Display a panel showing:

```
Total PII emails submitted: X
```

**Requirements:**
- Count must be retrieved from the backend
- Must persist across page reloads
- Must update after each submission

---

### 2. Backend

Implement the following processing pipeline for submitted text:

#### Detection
Identify all email addresses in the submitted text.

#### Classification
Assign the classification tag **`"pii.email"`** to each detected email address.

#### Tokenization
The key requirement is **tokenization for data protection**:
- Replace each detected email address with a unique token
- Each email must map to a distinct token
- This allows the text content to be safely processed and stored while maintaining structure
- Token format is your choice (e.g., `{{TKN-abc123}}`, `__emailToken1__`)

#### Persistence
Design your data storage to support the tokenization model:

1. **Submission Record**: Store the tokenized version of the text (emails replaced by tokens)
2. **Classification Records**: Store metadata about detected sensitive data (one record per email):
   - The unique token (links back to the tokenized text)
   - The original email value (for the vault/classification store)
   - The classification tag (`"pii.email"`)

This separation allows you to handle sensitive values differently from the tokenized content.

#### API Endpoints
Provide an endpoint that returns:
- The total count of all PII email items submitted across all submissions

**Implementation:** You are free to structure the backend as you see fit (Minimal APIs, MVC, layered architecture, etc.).

---

### 3. Database

#### Database Selection
Use any relational database (SQLite, PostgreSQL, SQL Server, etc.).

#### Data Model
Design a schema that supports the tokenization architecture:
- **Tokenized submissions**: Store the processed text with tokens in place of sensitive data
- **Classification vault**: Store the detected sensitive items with their tokens and metadata

This approach separates the usage context (tokenized text) from the sensitive values (classification records).

Schema design is your choice. Prioritize clarity and maintainability.

---

## Optional Extension: Multiple Classification Types

**This is entirely optional.** If you choose to implement it, extend the system to support multiple types of sensitive data beyond email addresses.

### Requirements

Allow configuration of multiple classification types, where each type includes:
- A tag name
- A detection mechanism (implementation approach is your choice)

**Example classification types:**

| Tag Name         | Data Type       |
|------------------|-----------------|
| `finance.iban`   | IBAN numbers    |
| `pii.phone`      | Phone numbers   |
| `security.token` | API keys/tokens |

### Implementation

The backend should:
1. Detect all configured data types in the submitted text
2. Tokenize each detected match (similar to email handling)
3. Persist classifications with their appropriate tags

### Configuration Management

You do **not** need to build a UI for managing classification rules. Acceptable approaches include:
- Hard-coded configuration
- JSON configuration files
- Database-seeded rules

**If you implement this extension, document your approach and design decisions in your submission README.**

---

## Evaluation Criteria

### What We Value

- **Clear thinking**: Logical approach to problem-solving
- **Code quality**: Clean, maintainable backend implementation
- **Correctness**: Accurate tokenization logic and data detection
- **Functional UI**: Real-time highlighting that works as specified
- **End-to-end integration**: Complete, working data flow from frontend to database
- **Pragmatic decisions**: Sensible trade-offs appropriate for the scope
- **Communication**: Willingness to ask questions and seek clarification when needed
- **Deployment**: Working Docker Compose setup

### What We Do NOT Expect

- Polished visual design or styling
- Production-grade error handling or edge case coverage
- Comprehensive test suites (though welcome if you choose to include them)
- Enterprise architecture patterns or overengineering

### Time Investment

This challenge is designed to be completed in **a few focused hours**. It is not intended to be a weekend-long project.

---

## Deployment

### Docker Compose Setup

Your submission must include a Docker Compose configuration that orchestrates:
- Backend service
- Database service
- Frontend service

Running the following command should start the complete application:

```bash
docker compose up
```

---

## Submission

### Required Deliverables

Please submit all the the following via a public Github repository that we can access:

1. **Source code**
   - Frontend implementation
   - Backend implementation

2. **Docker Compose configuration**
   - Complete setup to run all services

3. **Documentation (README)**
   - Instructions for running the application
   - Architectural decisions and assumptions
   - Trade-offs or shortcuts taken
   - (Optional) Implementation notes for the optional extension

---

## Closing Notes

We prioritize understanding your thought process and problem-solving approach over feature completeness. Focus on demonstrating clear thinking and sound engineering judgment within the specified scope.

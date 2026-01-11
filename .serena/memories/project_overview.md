# Project Overview: Logos API

## Purpose
Logos API is a RESTful web API that provides comprehensive Bible verse lookup functionality with deep Greek/Hebrew lexical data, morphology, semantic domains, and linguistic analysis. The API is designed to power exegetical Bible study applications, particularly for New Testament Greek analysis.

## Primary Features
- **Verse Lookup**: Retrieve verse data with full tokenization
- **Lexicon Data**: Strong's concordance numbers with definitions
- **Morphology**: Greek morphological parsing (RMAC codes)
- **Semantic Domains**: Louw-Nida semantic domain classifications
- **Word Frequency**: Hapax legomena detection and frequency analysis
- **Discourse Features**: Clause fronting and word order analysis
- **Related Words**: Semantic domain-based word relationships
- **Commentary Integration**: External commentary data via HelloAO API

## Target Use Cases
- AI-powered exegetical assistants (as shown in smart-exegetical-assistant-instructions.md)
- Bible study applications requiring deep linguistic analysis
- Greek/Hebrew word study tools
- Theological research platforms

## Deployment
- **Platform**: Koyeb (cloud container platform)
- **Docker**: Containerized deployment with embedded data resources
- **Environment**: Production API with Swagger UI enabled for testing
- **Health Checks**: `/health` and `/_health` endpoints for monitoring
- **Privacy**: Includes `/privacy` endpoint for ChatGPT GPT Store compliance

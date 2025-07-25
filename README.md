# 🎯 GitHub Portfolio Service
> A powerful .NET Core Web API for dynamic GitHub portfolio data integration

[![.NET](https://img.shields.io/badge/.NET-9.0+-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12+-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Octokit](https://img.shields.io/badge/Octokit.NET-Latest-blue?logo=github)](https://github.com/octokit/octokit.net)

**Transform your GitHub repositories into dynamic portfolio data with intelligent caching and advanced search capabilities!**

## 🌟 Features

- 🚀 **Dynamic Portfolio Data** - Real-time repository information for portfolio websites
- 🔍 **Repository Search** - Search public repositories by name, language, and user
- 🏎️ **Smart Caching** - In-memory caching with intelligent update detection
- 🔐 **Secure Authentication** - GitHub Personal Access Tokens via User Secrets
- 📊 **Rich Metadata** - Languages, stars, PRs, commit dates, and live links
- ⚡ **High Performance** - Optimized API calls and efficient caching

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- GitHub Personal Access Token
- Visual Studio 2022 or VS Code

### Installation & Setup
```bash
# Clone the repository
git clone https://github.com/Batya19/dotnet-github-portfolio-service.git
cd dotnet-github-portfolio-service

# Set up your GitHub token
dotnet user-secrets set "GitHub:Token" "your-github-token-here"

# Run the application
dotnet run --project GitHubPortfolio.Api
```

### Basic Usage
```bash
# Get your portfolio data
GET /api/github/portfolio

# Search repositories  
GET /api/github/search?query=dotnet&language=csharp
```

## 📖 API Endpoints

### Get Portfolio Data
```http
GET /api/github/portfolio
```
Returns your repositories with metadata for portfolio display.

### Search Repositories
```http
GET /api/github/search?query={query}&language={language}&user={user}
```
Search public repositories with optional filters.

**Parameters:**
- `query` - Repository name search term
- `language` - Programming language filter  
- `user` - GitHub username filter

## 🏗️ Architecture

### Project Structure
```
📦 GitHubPortfolio
├── 📄 GitHubPortfolio.sln             # Solution file
├── 📄 README.md                       # Project documentation
├── 📄 .gitignore                      # Git ignore rules
├── 📁 GitHubPortfolio.Api/            # Web API Project
│   ├── 📄 Program.cs                  # Application entry point
│   ├── 📄 appsettings.json            # Configuration
│   ├── 📄 GitHubPortfolio.Api.http    # HTTP test file
│   ├── 📁 Controllers/                # API endpoints
│   │   └── 📄 GitHubController.cs     # Main controller
│   └── 📁 Properties/                 # Launch settings
│       └── 📄 launchSettings.json
└── 📁 GitHubPortfolio.Service/        # Class Library
    ├── 📄 GitHubPortfolio.Service.csproj
    ├── 📁 Services/                   # Business logic
    │   ├── 📄 GitHubService.cs        # Core GitHub integration
    │   └── 📄 IGitHubService.cs       # Service interface
    ├── 📁 Models/                     # Data models
    │   ├── 📄 GitHubPortfolioOptions.cs
    │   ├── 📄 RepositoryInfo.cs
    │   └── 📄 SearchOptions.cs
    └── 📁 Cache/                      # Caching layer
        ├── 📄 ICacheService.cs        # Cache interface
        └── 📄 InMemoryCacheService.cs # Cache implementation
```

### Key Components

**GitHubService (Service Library)**
- Handles GitHub API integration using Octokit.NET
- Manages authentication and rate limiting
- Provides repository data extraction and search functionality

**GitHubController (Web API)**
- Exposes REST endpoints for portfolio data
- Implements smart caching strategy via ICacheService
- Handles search functionality and error management

**Caching Layer**
- ICacheService interface for flexible caching strategies
- InMemoryCacheService implementation for fast data access
- Configurable cache expiration and intelligent updates

## 🔧 Configuration

### GitHub Token Setup
```bash
# Set your GitHub Personal Access Token
dotnet user-secrets set "GitHub:Token" "ghp_your_token_here"
```

**Required token permissions:**
- `public_repo` - Access to public repositories
- `user` - Access to user profile information

## 🎯 Use Cases

- 💼 **Portfolio Websites** - Display live GitHub project data
- 📊 **Developer Dashboards** - Monitor repository statistics  
- 🔍 **Project Discovery** - Explore and search repositories

## 🛠️ Technical Details

- **Framework:** .NET 9.0 Web API
- **GitHub Integration:** Octokit.NET library
- **Caching:** In-memory caching with expiration
- **Security:** User Secrets for token management
- **Performance:** Optimized API calls and data caching

## 💡 Smart Caching Strategy

The service implements intelligent caching that:
- Checks for repository updates before serving cached data
- Expires cache based on configurable timeouts
- Minimizes GitHub API rate limit usage
- Ensures fresh data for portfolio displays

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 🐛 Troubleshooting

**Common Issues:**

**401 Unauthorized**
- Verify your GitHub token is set: `dotnet user-secrets list`
- Check token permissions and expiration date

**Rate Limit Exceeded**
- The service uses authenticated requests for higher limits
- Consider implementing delays between requests if needed

```bash
# Check your configuration
dotnet user-secrets list

# Restart to clear cache
dotnet run --project GitHubPortfolio.Api
```

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

## 👩‍💻 Author

**Batya Zilberberg** - [GitHub](https://github.com/Batya19)

---

<div align="center">

**Made with 💖 by a developer who believes in dynamic portfolios**

*Connect your GitHub, power your portfolio! 🚀✨*

</div>
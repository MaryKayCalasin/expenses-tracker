# 💸 AI Expenses Tracker

A smart backend API that automatically categorises your expenses using AI.
Built with C# and ASP.NET Core by a nurse transitioning to backend development.

## ✨ Features

- 📸 **AI Receipt Parser** — paste any receipt text and AI automatically extracts the amount, category and description
- 📊 **Spending Summary** — see your total spending grouped by category
- ➕ **Add Expenses** — manually log any expense
- 🗑️ **Delete Expenses** — remove incorrect entries
- 🇳🇴 **Built for Norway** — categories designed for Norwegian healthcare workers (Uniform, Transport, Medical)

## 🚀 Live Demo

**API:** https://expenses-tracker-production-75bf.up.railway.app/swagger

Try it yourself — go to the link above, click **POST /api/Ai/parse-receipt → Try it out** and paste any receipt text like:


## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# 10 |
| Framework | ASP.NET Core |
| Database | SQLite + Entity Framework Core |
| AI | OpenRouter (Llama 3) |
| Deployment | Railway |
| Version Control | Git + GitHub |

## 📡 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/expenses | Get all expenses |
| POST | /api/expenses | Add a new expense |
| DELETE | /api/expenses/{id} | Delete an expense |
| GET | /api/expenses/summary | Monthly summary by category |
| POST | /api/ai/parse-receipt | Parse receipt text with AI |

## 🏃 Run Locally

```bash
# Clone the repo
git clone https://github.com/MaryKayCalasin/expenses-tracker.git
cd expenses-tracker

# Add your OpenRouter API key to appsettings.json
# Run database migrations
dotnet ef database update

# Start the app
dotnet run
```

Then open http://localhost:5210/swagger

## 👩‍⚕️ About

Built by Mary Kay Calasin — a registered nurse transitioning into backend development.
This project is part of a portfolio targeting healthcare IT roles in Norway.


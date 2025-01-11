# Probot
A comprehensive subscription payment system leveraging the Solana blockchain, integrated into a Discord bot as the client interface.
Probot facilitates the purchase and management of subscriptions for various cryptocurrency tools in an efficient and user-friendly manner. This project is divided into two main parts:
1. Client (located in the Client folder)
2. Backend (includes SubscriptionsAPI, Data, and ProRaffleTool folders - the first tool)

## Client
The client application was developed in C# using the DSharpPlus library. It can be installed on any Discord server and serves as the GUI for users to purchase subscriptions. Subscriptions provide access to cryptocurrency tools that offer meaningful advantages in the crypto world.<br>
Key Features:
- Subscribe, allows users to subscribe to different plans for a product.
![subscribeflow](https://github.com/user-attachments/assets/dc97adf3-382a-41e9-922c-f6f0491e2a2f)

- Payment Wallets, configure the payment address for the Solana blockchain.
![PaymentWalletsFlow](https://github.com/user-attachments/assets/2343a9ce-5689-4258-87c0-9ad483a80d50)

- Product Details, view details of available products.
![ProductDetailsFlow](https://github.com/user-attachments/assets/4f509fa6-e3c0-426e-9190-a39c813d1c41)

- Claim code, generate claim codes to provide access to different communities or for trial versions.
![ClaimCodeFlow](https://github.com/user-attachments/assets/e4998a37-be49-48c5-aad8-cd02ee756c4a)

## Client - Available commands accessible to users with active subscriptions:
- /product-keys - View product keys associated with your account.
- /new-subscription - Start a new subscription.
- /extend-subscription - Extend an existing subscription.
- /update-alphabot-key - Update your Alphabot key.
- /bot-status - Check the status of the bot.

All operations communicate with the backend through the SubscriptionsAPI.

## Backend
The backend system is divided into multiple components that work together to handle subscriptions, orders, and related functionalities.

### Main Components
#### SubscriptionsAPI
- Manages subscription-related operations and communicates with the client.
- Follows the **MVC pattern** (Model-View-Controller).
- Provides multiple endpoints for managing:
  - Users
  - Orders
  - Subscriptions
  - Products
  - Product keys
- Is secured by an **API key**, ensuring access is restricted to authorized users only.
- Contains 3 background jobs for efficient task handling. Jobs 1 and 2 remain active but idle until triggered by the arrival of a new order:
  - TransactionMonitorService, searches **Solana blockchain (RPC Call)** for newly added transaction for a period of 10min and if found notifies OrderMonitorService using a transaction channel
  - OrderMonitorService, processes orders, updates their states, and notifies users of changes in real-time using **SignalR**.
  - SubscriptionCheckService, sends reminders to authorized users about upcoming subscription expirations. Notifications are sent at 7, 3, and 1 day(s) before expiration, also utilizing **SignalR** for real-time alerts.
- Utilizes **Entity Framework (Code-First Approach)** and **LINQ** to manage and communicate with the database, for all the operations mentioned above.

#### Data
- Contains all database-related objects, configurations and migrations.
- **SQLite** is used to store all the data.

## ProRaffleTool
**Problem** <br>
 Every day, approximately 1,000 raffles are created. Managing these raffles manually would require significant time and effort, especially when using multiple accounts.<br><br>
**Solution** <br>
 An automated system was developed to enhance clients' chances of winning whitelist spots in NFT raffles. This tool complements the subscription system and provides users with a significant competitive edge.<br>
 
 #### Example of a raffle and related tasks
![AlphabotTasks](https://github.com/user-attachments/assets/0cf67b66-3b8f-4025-a578-6e2b66d0e240)

#### Key Features:
1. Webhook Integration
   - Receives and processes new raffle.
   - Registers users in raffles through the Alphabot.app API.
   - Utilizes Ngrok to safely expose the webhook to external sources.

2. Automated Raffle Registration
    - A background job runs every 2 hours to automatically register users for leftover raffles, maximizing participation opportunities.

3. Discord OAuth2 Implementation - allow users to monitor in real time which raffles were successfully registered and which ones failed. The user must give authorization to attach a webhook in a specific server in order to work.
![image](https://github.com/user-attachments/assets/34a0a629-61e5-41b6-8979-9fa32b258250)


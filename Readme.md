## To what extent can Homomorphic Encryption be beneficial for the Health Care Sector? 
This is the repository of the demo calories calculator. Click on the screenshot to see it in action.

## Solution Structure
The solution **CybersecHSG.sln** references two projects. In the **CybersecHSG.Common** class library the Models are found. These classes are used from the client and the server. The **CybersecHSG.ClientServer** web project contains both the Blazor Server app and the Web API. The logic for storing the users selected activities, making the requests to the API and then decrypting its responses is found in the SEAL Razor Component in the folder components. The code for handling the received requests to the API is found under /Controllers/CalculationController.

[![SC2 Video](CybersecHSG.ClientServer/Screenshot.PNG)](https://youtu.be/zKyUQ5atFUw "Calories Calculator - Click to Watch!")

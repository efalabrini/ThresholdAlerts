import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { ThemeProvider } from '@mui/material/styles';
import { theme } from "./styles/theme";

//Azure Insights imports
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import { createBrowserHistory } from "history";

// MSAL imports
import { PublicClientApplication, EventType } from "@azure/msal-browser";
import { msalConfig } from "./authConfig";

export const msalInstance = new PublicClientApplication(msalConfig);


msalInstance.initialize().then(() => {
  // Default to using the first account if no account is active on page load
  if (!msalInstance.getActiveAccount() && msalInstance.getAllAccounts().length > 0) {
    // Account selection logic is app dependent. Adjust as needed for different use cases.
    msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0]);
  }

  // Optional - This will update account state if a user signs in from another tab or window
  msalInstance.enableAccountStorageEvents();

  msalInstance.addEventCallback((event) => {
    if (event.eventType === EventType.LOGIN_SUCCESS
      ||
      event.eventType === EventType.ACQUIRE_TOKEN_SUCCESS
      ||
      event.eventType === EventType.SSO_SILENT_SUCCESS
    ) {
      const account = event.payload.account;
      msalInstance.setActiveAccount(account);
    }
  });

//Implement Azure Insights
const appinsightsConnectionString = process.env.REACT_APP_THRESHOLDALERTS_APPINSIGHTS_CONNECTIONSTRING;
//console.log('Application Insights Connection String:', appinsightsConnectionString);
if (appinsightsConnectionString) {
  try {
    console.log('Start appinsight instrumentation');
    const browserHistory = createBrowserHistory({ basename: '' });
    var reactPlugin = new ReactPlugin();
    // *** Add the Click Analytics plug-in. ***
    /* var clickPluginInstance = new ClickAnalyticsPlugin();
      var clickPluginConfig = {
        autoCapture: true
    }; */
    var appInsights = new ApplicationInsights({
        config: {
            connectionString: appinsightsConnectionString,
            // *** If you're adding the Click Analytics plug-in, delete the next line. ***
            extensions: [reactPlugin],
        // *** Add the Click Analytics plug-in. ***
        // extensions: [reactPlugin, clickPluginInstance],
            extensionConfig: {
              [reactPlugin.identifier]: { history: browserHistory }
          // *** Add the Click Analytics plug-in. ***
          // [clickPluginInstance.identifier]: clickPluginConfig
            }
        }
    });
    appInsights.loadAppInsights();
    console.log('Finish appinsight instrumentation');
  } catch (error) {
    console.log('Error starting appinsight instrumentation:', error);
  }
}
// End Azure Insights implementation

  const root = ReactDOM.createRoot(document.getElementById('root'));
  root.render(
    <React.StrictMode>
      <ThemeProvider theme={theme}>
        <App pca={msalInstance} />
      </ThemeProvider>
    </React.StrictMode>
  );

});

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

import './App.css';
import MeasurementList from './components/MeasurementList'; // Adjust the path if necessary
import MySubscriptions from './components/MySubscriptions';
import { AuthenticatedTemplate, UnauthenticatedTemplate, MsalProvider } from "@azure/msal-react";
import Typography from "@mui/material/Typography";
import { PageLayout } from "./components/PageLayout";
import Readings from './components/Readings';
import { useState } from 'react';

function App({ pca }) {

  const [reloadSubscriptions, setReloadSubscriptions] = useState(false);

  const handleReloadSubscriptions = () => {
    setReloadSubscriptions((prev) => !prev); // Toggle state to trigger re-render
  };

  return (
    <div className="App">
      <MsalProvider instance={pca}>
        <PageLayout>
          <AuthenticatedTemplate>
            <MeasurementList onSubscriptionAdded={handleReloadSubscriptions} />
            <Readings />
            <MySubscriptions key={reloadSubscriptions} />
          </AuthenticatedTemplate>
          
          <UnauthenticatedTemplate>
            <MeasurementList />
            <Readings />
            <Typography variant="h6">
              <br/>
              <center>Please sign-in to see your subscriptions.</center>
            </Typography>
          </UnauthenticatedTemplate>
        </PageLayout>
      </MsalProvider>
    </div>
  );
}

export default App;

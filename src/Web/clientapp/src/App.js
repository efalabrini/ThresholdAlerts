import './App.css';
import MeasurementList from './components/MeasurementList'; // Adjust the path if necessary
import MySubscriptions from './components/MySubscriptions';
import { AuthenticatedTemplate, UnauthenticatedTemplate, MsalProvider } from "@azure/msal-react";
import Typography from "@mui/material/Typography";
import { PageLayout } from "./components/PageLayout";

function App({ pca }) {
  return (
    <div className="App">
      <MsalProvider instance={pca}>
        <PageLayout>
          <AuthenticatedTemplate>
            <MeasurementList />
            <MySubscriptions />
          </AuthenticatedTemplate>
          
          <UnauthenticatedTemplate>
            <MeasurementList />
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

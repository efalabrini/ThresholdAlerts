import React, { useEffect, useState } from 'react';
import { loginRequest } from "../authConfig";
import { useMsal } from "@azure/msal-react";

const MySubscriptions = () => {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { instance } = useMsal();
  
    useEffect(() => {
      const fetchData = async () => {

        const account = instance.getActiveAccount();
        if (!account) {
            throw Error("No active account! Verify a user has been signed in and setActiveAccount has been called.");
        }
    
        const response = await instance.acquireTokenSilent({
            ...loginRequest,
            account: account
        });
        const accessToken = response.accessToken;

        const apiUrl = process.env.REACT_APP_API_URL  + 'api/User/me/subscriptions';

        try {
          const response = await fetch(apiUrl, {
            method: 'GET',
            headers: {
                'Authorization' : `Bearer ${accessToken}`,
                'Content-type' : 'application/json'
            }
          });
          if (!response.ok) {
            throw new Error('Network response was not ok');
          }
          const result = await response.json();
          setData(result);
        } catch (error) {
          setError(error.message);
        } finally {
          setLoading(false);
        }
      };
  
      fetchData();
    },[]);
  
    if (loading) {
      return <p>Loading...</p>;
    }
  
    if (error) {
      return <p>Error: {error}</p>;
    }

    const handleDelete = async (measurementId) => {    
      const account = instance.getActiveAccount();
        if (!account) {
            throw Error("No active account! Verify a user has been signed in and setActiveAccount has been called.");
        }
    
        const response = await instance.acquireTokenSilent({
            ...loginRequest,
            account: account
        });
        const accessToken = response.accessToken;

        const apiUrl = process.env.REACT_APP_API_URL  +  `api/User/me/subscription?measurementId=${measurementId}`;

      try {
        const response = await fetch(
          apiUrl,
          {
            method: 'DELETE',
            headers: {
                'Authorization' : `Bearer ${accessToken}`,
                'Content-type' : 'application/json'
            }
          }
        );
    
        if (response.ok) {
          // Optionally, update the local data to remove the deleted subscription.
          setData((prevData) => prevData.filter((item) => item.measurementId !== measurementId));
        } else {
          alert("Failed to delete the subscription.");
        }
      } catch (error) {
        console.error("Error deleting subscription:", error);
        alert("An error occurred while trying to delete the subscription.");
      }
    };
  
    return (
    <div>
      <h2>My Subscriptions</h2>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Measurement</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Unit</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Lower Threshold</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Upper Threshold</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {data.map((item, index) => (
            <tr key={index}>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{item.measurement}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{item.measurementUnit}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{new Intl.NumberFormat('en-US').format(item.lowerThreshold)}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{new Intl.NumberFormat('en-US').format(item.upperThreshold)}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'center' }}>
                <button
                  onClick={() => handleDelete(item.measurementId)}
                  style={{ padding: '5px 10px', color: 'white', backgroundColor: 'red', border: 'none', cursor: 'pointer' }}
                >
                  Delete
                </button>
            </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default MySubscriptions;

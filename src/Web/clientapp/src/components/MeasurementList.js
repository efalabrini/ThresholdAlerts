import React, { useEffect, useState } from 'react';
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../authConfig";

const MeasurementList = ({ onSubscriptionAdded }) => {

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const apiUrl = process.env.REACT_APP_API_URL  + 'api/Measurement';
  const [isModalOpen, setModalOpen] = useState(false);
  const [currentId, setCurrentId] = useState(null);
  const [thresholds, setThresholds] = useState({
    lowerThreshold: '',
    upperThreshold: '',
  });
  const { instance } = useMsal();

  console.log(apiUrl);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch(apiUrl);
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

  const handleAddSubscription = (id) => {
    setCurrentId(id);
    setModalOpen(true);
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setThresholds({ lowerThreshold: '', upperThreshold: '' });
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setThresholds((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async () => {
    const { lowerThreshold, upperThreshold } = thresholds;

    try {

      const account = instance.getActiveAccount();
        if (!account) {
            throw Error("No active account! Verify a user has been signed in and setActiveAccount has been called.");
        }

      const responselog = await instance.acquireTokenSilent({
            ...loginRequest,
            account: account
        })
      const accessToken = responselog.accessToken;
      const apiUrl = process.env.REACT_APP_API_URL  + `api/User/me/subscription?measurementId=${currentId}`;
      const response = await fetch(
        apiUrl,
        {
          method: 'PUT',
          headers: {
            'accept': 'text/plain',
            'Authorization': `Bearer ${accessToken}`, // Replace with the actual token
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            lowerThreshold: Number(lowerThreshold),
            upperThreshold: Number(upperThreshold),
          }),
        }
      );

      if (response.ok) {
        onSubscriptionAdded(); // Notify App.js to reload MySubscriptions
      } else {
        const errorText = await response.text();
        alert(`Failed to add subscription: ${errorText}`);
      }
    } catch (error) {
      console.error('Error adding subscription:', error);
      alert('An error occurred while adding the subscription.');
    } finally {
      handleCloseModal();
    }
  };

  return (
    <div>
      <h2>Measurements</h2>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Name</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Unit</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>API URL</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Subscription</th>
          </tr>
        </thead>
        <tbody>
          {data.map((entity) => (
            <tr key={entity.id}>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.name}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.unit}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>
                <a href={entity.apiUrl} target="_blank" rel="noopener noreferrer">{entity.apiUrl}</a>
              </td>
              <td style={{ border: '1px solid #ddd', padding: '8px', textAlign: 'center' }}>
                <button onClick={() => handleAddSubscription(entity.id)}>Add</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {isModalOpen && (
        <div
          style={{
            position: 'fixed',
            top: '50%',
            left: '50%',
            transform: 'translate(-50%, -50%)',
            padding: '20px',
            backgroundColor: 'white',
            border: '1px solid #ddd',
            borderRadius: '8px',
            zIndex: 1000,
          }}
        >
          <h3>Set Thresholds</h3>
          <label>
            Lower Threshold:
            <input
              type="number"
              name="lowerThreshold"
              value={thresholds.lowerThreshold}
              onChange={handleInputChange}
              style={{ margin: '10px 0', display: 'block', width: '100%' }}
            />
          </label>
          <label>
            Upper Threshold:
            <input
              type="number"
              name="upperThreshold"
              value={thresholds.upperThreshold}
              onChange={handleInputChange}
              style={{ margin: '10px 0', display: 'block', width: '100%' }}
            />
          </label>
          <button onClick={handleSubmit} style={{ marginRight: '10px' }}>
            Submit
          </button>
          <button onClick={handleCloseModal}>Cancel</button>
        </div>
      )}

      {isModalOpen && (
        <div
          style={{
            position: 'fixed',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            zIndex: 999,
          }}
          onClick={handleCloseModal}
        />
      )}

    </div>
  );
};

export default MeasurementList;

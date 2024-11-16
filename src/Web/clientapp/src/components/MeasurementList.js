import React, { useEffect, useState } from 'react';

const MeasurementList = () => {

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const apiUrl = process.env.REACT_APP_API_URL  + '/api/Measurement';

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
  }, []);

  if (loading) {
    return <p>Loading...</p>;
  }

  if (error) {
    return <p>Error: {error}</p>;
  }


  return (
    <div>
      <h2>Measurements</h2>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Name</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Unit</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>API URL</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Field JSON Path</th>
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
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.fieldJsonPath}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default MeasurementList;

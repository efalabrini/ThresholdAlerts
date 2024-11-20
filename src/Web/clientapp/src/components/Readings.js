import React, { useEffect, useState } from 'react';

const Readings = () => {

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const apiUrl = process.env.REACT_APP_API_URL  + 'api/Alert/ListMeasurementReadings';

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


  return (
    <div>
      <h2>Readings (Date format dd/mm/yyyy hh:mm)</h2>
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Measurement</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Unit</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Value</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Read at</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Min</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Read at</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Max</th>
            <th style={{ border: '1px solid #ddd', padding: '8px' }}>Read at</th>
          </tr>
        </thead>
        <tbody>
          {data.map((entity) => (
            <tr key={entity.id}>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.measurement}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.unit}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{new Intl.NumberFormat('en-US').format(entity.value)}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.readAt
               ? new Intl.DateTimeFormat('en-GB', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                hour12: false,
                }).format(new Date(entity.readAt))
                : '' }
              </td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{new Intl.NumberFormat('en-US').format(entity.minValue)}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.minValueReadAt
                ? new Intl.DateTimeFormat('en-GB', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: false,
                    }).format(new Date(entity.readAt))
                    : '' }</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{new Intl.NumberFormat('en-US').format(entity.maxValue)}</td>
              <td style={{ border: '1px solid #ddd', padding: '8px' }}>{entity.maxValueReadAt
                ? new Intl.DateTimeFormat('en-GB', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: false,
                    }).format(new Date(entity.readAt))
                    : '' }</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Readings;
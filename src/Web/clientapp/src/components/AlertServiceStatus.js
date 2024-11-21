import React, { useEffect, useState } from 'react';

const AlertServiceStatus = () => {
  const [status, setStatus] = useState(null); // `null` for initial loading state
  const [error, setError] = useState(null);
  const [period, setPeriod] = useState(null); // For `periodInMinutes`
  const [startTime, setstartTime] = useState(null);

  useEffect(() => {
    const fetchStatus = async () => {
      try {
        const apiUrl = process.env.REACT_APP_API_URL  + 'api/ConfigInfo/alertservice/status';
        const response = await fetch(apiUrl, {
          headers: { accept: 'text/plain' },
        });
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        setStatus(data.isRunning);
        setPeriod(data.periodInMinutes);
        setstartTime(data.startTime);
      } catch (err) {
        setError(err.message);
      }
    };

    fetchStatus();
  }, []);

  if (error) {
    return <div>Error: {error}</div>;
  }

  if (status === null) {
    return <div>Loading...</div>;
  }

  const textStyle = {
    color: status ? 'green' : 'red',
    fontSize: '12px',
    textAlign: 'left',
    paddingLeft: '10px'
  };

  function formatReadAtDate(readAt) {
    return readAt
        ? new Intl.DateTimeFormat('en-GB', {
              day: '2-digit',
              month: '2-digit',
              year: 'numeric',
              hour: '2-digit',
              minute: '2-digit',
              hour12: false,
          }).format(new Date(readAt))
        : '';
  }
  return <div style={textStyle}>{status ? `Alert Service running every ${period} minutes (Started at ${formatReadAtDate(startTime)})` : 'Alert Service stopped'}</div>;
};

export default AlertServiceStatus;
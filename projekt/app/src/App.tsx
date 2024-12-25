import React from 'react';
import { useRoutes } from 'react-router-dom'; // Import useRoutes
import Login from './pages/Login';

const App: React.FC = () => {
  const routes = [
    { path: "/login", element: <Login /> }
  ];

  const element = useRoutes(routes);

  return element;
};

export default App;

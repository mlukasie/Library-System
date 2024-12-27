import React from 'react';
import { useRoutes } from 'react-router-dom';
import Login from './pages/Login';
import LibrarianDashboard from './pages/LibrarianDashboard';
import UserDashboard from './pages/UserDashboard';

const App: React.FC = () => {
  const routes = [
    { path: "/Login", element: <Login /> },
    { path: "/Librarian-dashboard", element:<LibrarianDashboard />},
    { path: "/User-dashboard", element:<UserDashboard />},
    { path: "/unauthorized", element: <div>Unauthorized Access</div> },
  ];

  const element = useRoutes(routes);

  return element;
};

export default App;

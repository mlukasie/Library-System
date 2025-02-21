import React from 'react';
import { useRoutes } from 'react-router-dom';
import Login from './pages/Login';
import LibrarianDashboard from './pages/LibrarianDashboard';
import UserDashboard from './pages/UserDashboard';
import EditBook from './pages/EditBook';
import DetailsBook from './pages/DetailsBook';
import DeleteBook from './pages/Delete';
import CreateBook from './pages/CreateBook';
import Register from './pages/Register';
import UserReservations from './pages/UserReservations';
import LibrarianReservations from './pages/LibrarianReservations';
import UserLeases from './pages/UserLeases';
import LibrarianLeases from './pages/LibrarianLeases';
import UserDetails from './pages/UserDetails';

const App: React.FC = () => {
  const routes = [
    { path: "/Login", element: <Login /> },
    { path: "/Librarian-dashboard", element:<LibrarianDashboard />},
    { path: "/User-dashboard", element:<UserDashboard />},
    { path: "/unauthorized", element: <div>Unauthorized Access</div> },
    { path: "/Edit/:id", element:<EditBook />},
    { path: "/Details/:id", element:<DetailsBook />},
    { path: "/Delete/:id", element:<DeleteBook />},
    { path: "/Create-book", element:<CreateBook />},
    { path: "/Register", element:<Register />},
    { path: "/User-Reservations", element:<UserReservations />},
    { path: "/Librarian-Reservations", element:<LibrarianReservations />},
    { path: "/User-Leases", element:<UserLeases />},
    { path: "/Librarian-Leases", element:<LibrarianLeases />},
    { path: "/User-Details", element:<UserDetails />},
  ];

  const element = useRoutes(routes);

  return element;
};

export default App;

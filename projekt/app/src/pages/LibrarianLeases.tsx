import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import CONFIG from '../config';

interface Lease {
  id: number;
  bookId: number;
  bookTitle: string;
  userEmail: string;
  leaseDate: string;
  isActive: boolean;
}

const LibrarianLeases: React.FC = () => {
  const [leases, setLeases] = useState<Lease[]>([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchLeases = async () => {
      try {
        const response = await fetch(`${CONFIG.API_URL}/api/Librarian/Leases`, {
            credentials: 'include',});
        if (response.status === 401 || response.status === 403) {
          navigate('/unauthorized');
          return;
        }
        if (!response.ok) {
          throw new Error('Failed to fetch leases');
        }
        const data = await response.json();
        setLeases(data);
      } catch (error: any) {
        console.error('Error fetching leases:', error.message);
        setErrorMessage(error.message || 'Failed to load leases.');
      } finally {
        setLoading(false);
      }
    };

    fetchLeases();
  }, [navigate]);

  const handleChangeStatus = async (leaseId: number) => {
    try {
      const response = await fetch(`${CONFIG.API_URL}/api/Lease/ChangeStatus/${leaseId}`, {
        method: 'PUT',
        credentials: 'include',
      });
      if (!response.ok) {
        throw new Error('Failed to update lease status');
      }

      setLeases((prevLeases) =>
        prevLeases.map((lease) =>
          lease.id === leaseId ? { ...lease, isActive: false } : lease
        )
      );
      alert('Lease status updated successfully!');
    } catch (error: any) {
      console.error('Error updating lease status:', error.message);
      setErrorMessage(error.message || 'Failed to update lease status.');
    }
  };

  const handleLogout = async () => {
    try {
      const response = await fetch(`${CONFIG.API_URL}/api/Account/logout`, {
        method: 'POST',
        credentials: 'include',
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Logout failed');
      };
      navigate('/Login');
    }
    catch (error: any) {
      setErrorMessage(error.message || 'Logout failed. Please try again.');
    }
    
}

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
      <h2>
      <button
          className="btn btn-primary"
          onClick={() => navigate('/User-details')}
        >
          My Profile
        </button>
      </h2>
      <div>
          <button
            className="btn btn-primary me-2"
            onClick={() => navigate('/Librarian-Reservations')}
          >
            Reservations
          </button>
          <button
            className="btn btn-primary me-2"
            onClick={() => navigate('/Librarian-Leases')}
          >
            Leases
          </button>
          <button
            className="btn btn-primary me-2"
            onClick={() => navigate('/Librarian-Dashboard')}
          >
            Books
          </button>
          <button
            className="btn btn-danger"
            onClick={handleLogout}
          >
            Logout
          </button>
          {errorMessage && (
            <div className="alert alert-danger" role="alert">
                {errorMessage}
            </div>
          )}
        </div>
      </div>
      {errorMessage && <div className="alert alert-danger">{errorMessage}</div>}
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Book Title</th>
            <th>User Email</th>
            <th>Lease Date</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {leases.map((lease) => (
            <tr key={lease.id}>
              <td>{lease.bookTitle}</td>
              <td>{lease.userEmail}</td>
              <td>{new Date(lease.leaseDate).toLocaleString()}</td>
              <td>{lease.isActive ? 'Active' : 'Returned'}</td>
              <td>
                {lease.isActive && (
                  <button
                    className="btn btn-danger btn-sm"
                    onClick={() => handleChangeStatus(lease.id)}
                  >
                    Return Book
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default LibrarianLeases;

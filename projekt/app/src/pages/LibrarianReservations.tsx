import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import CONFIG from '../config';

interface Reservation {
  id: number;
  userId: number;
  bookId: number;
  reservationDate: string;
  bookTitle: string;
  userEmail: string;
}

const LibrarianReservations: React.FC = () => {
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [loading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchReservations = async () => {
      try {
        const response = await fetch(`${CONFIG.API_URL}/api/Librarian/Reservations`, {
          credentials: 'include',});
        if (response.status === 401 || response.status === 403) {
          navigate('/unauthorized');
          return;
        }
        if (!response.ok) {
          throw new Error('Failed to fetch reservations');
        }
        const data = await response.json();
        setReservations(data);
      } catch (error) {
        console.error('Error fetching reservations:', error);
        setErrorMessage('Failed to load reservations.');
      } finally {
        setLoading(false);
      }
    };

    fetchReservations();
  }, [navigate]);

  const handleLogout = async () => {
    try {
      const response = await fetch(`${CONFIG.API_URL}/api/Account/logout`, {
        method: 'POST',
        credentials: 'include',
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Logout failed.');
      }
      navigate('/Login');
    } catch (error: any) {
      setErrorMessage(error.message || 'Logout failed.');
    }
  };

  const handleCancel = async (reservationId: number) => {
    try {
      const response = await fetch(`${CONFIG.API_URL}/api/Reservation/${reservationId}`, {
        method: 'DELETE',
        credentials: 'include',
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to cancel reservation.');
      }

      setReservations((prevReservations) =>
        prevReservations.filter((reservation) => reservation.id !== reservationId)
      );
      alert('Reservation canceled successfully!');
    } catch (error: any) {
      console.error('Error canceling reservation:', error.message);
      setErrorMessage(error.message || 'Failed to cancel reservation.');
    }
  };

  const handleLease = async (reservationID: number) => {
    try {
      const response = await fetch(`${CONFIG.API_URL}/api/LeaseBook/${reservationID}`, {
        method: 'POST',
        credentials: 'include',
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to lease the book.');
      }
      setReservations((prevReservations) =>
        prevReservations.filter((reservation) => reservation.id !== reservationID)
      );
      alert('Book leased successfully!');
    } catch (error: any) {
      alert('Couln`t lease book')
      console.error('Error leasing book:', error.message);
      setErrorMessage(error.message || 'Failed to lease the book.');
    }
  };

  const handleDeleteInactiveReservations = async () => {
    try {
      const response = await fetch(`${CONFIG.API_URL}/api/InactiveReservations`, {
        method: 'DELETE',
        credentials: 'include',
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to delete inactive reservations.');
      }
      window.location.reload()
      alert('Inactive reservations deleted successfully!');
      setReservations((prevReservations) =>
        prevReservations.filter((reservation) => reservation.id)
      );
    } catch (error: any) {
      console.error('Error deleting inactive reservations:', error.message);
      setErrorMessage(error.message || 'Failed to delete inactive reservations.');
    }
  };

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
            onClick={() => handleDeleteInactiveReservations()}
          >
            Delete Inactive Reservations
          </button>
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
      {errorMessage && (
        <div className="alert alert-danger" role="alert">
          {errorMessage}
        </div>
      )}
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Book Title</th>
            <th>Reservation Date</th>
            <th>User Email</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {reservations.map((reservation) => (
            <tr key={reservation.id}>
              <td>
                <a
                  href={`/Details/${reservation.bookId}`}
                  className="text-primary"
                >
                  {reservation.bookTitle}
                </a>
              </td>
              <td>{new Date(reservation.reservationDate).toLocaleString()}</td>
              <td>{reservation.userEmail}</td>
              <td>
                <button
                  className="btn btn-success btn-sm me-2"
                  onClick={() => handleLease(reservation.id)}
                >
                  Lease Book
                </button>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => handleCancel(reservation.id)}
                >
                  Cancel
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default LibrarianReservations;

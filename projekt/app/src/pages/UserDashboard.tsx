import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import CONFIG from '../config';

interface Book {
  id: number;
  title: string;
  author: string;
  isAvailable: boolean;
}

const LibrarianDashboard: React.FC = () => {
  const [books, setBooks] = useState<Book[]>([]);
  const [errorMessage, setErrorMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        const response = await fetch(`${CONFIG.API_URL}/api/Books/UserBooks`, {
          credentials: 'include',});
        if (response.status === 401 || response.status === 403) {
          navigate('/unauthorized');
          return;
        }
        if (!response.ok) {
          throw new Error("Failed to fetch books");
        }
        const data = await response.json();
        setBooks(data);
      } catch (error) {
        console.error("Error fetching books:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchBooks();
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
        };
        navigate('/Login');
      }
      catch (error: any) {
        setErrorMessage(error.message || 'Logout failed.');
      };
  }

  const handleReservation = async (book_id: number) => {
    try {
      const response = await fetch(`${CONFIG.API_URL}/api/ReserveBook/${book_id}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include',
      });
  
      if (!response.ok) {
        const errorData = await response.json();
        alert('Something went wrong');
        throw new Error(errorData.message || 'Failed to reserve the book.');
        
      }
  
      setBooks((prevBooks) =>
        prevBooks.map((book) =>
          book.id === book_id ? { ...book, isAvailable: false } : book
        )
      );
      alert('Book reserved successfully!');
    } catch (error: any) {
      console.error('Error reserving book:', error.message);
      setErrorMessage(error.message || 'Failed to reserve the book.');
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
            onClick={() => navigate('/User-Reservations')}
          >
            Reservations
          </button>
          <button
            className="btn btn-primary me-2"
            onClick={() => navigate('/User-Leases')}
          >
            Leases
          </button>
          <button
            className="btn btn-primary me-2"
            onClick={() => navigate('/User-dashboard')}
          >
            Books
          </button>
          <button
            className="btn btn-danger"
            onClick={handleLogout}
          >
            Logout
          </button>
        </div>
      </div>
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Title</th>
            <th>Author</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {books.map((book) => (
            <tr key={book.id}>
              <td>{book.title}</td>
              <td>{book.author}</td>
              <td>
                <button
                  className={`btn btn-${book.isAvailable ? 'success' : 'secondary'} btn-sm`}
                  disabled={!book.isAvailable}
                  onClick={() => handleReservation(book.id)}
                >
                  {book.isAvailable ? "Reserve" : "Not Available"}
                </button>
                | 
                <a href={`/Details/${book.id}`} className="btn btn-secondary btn-sm">Details</a>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default LibrarianDashboard;

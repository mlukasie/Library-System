import React, { useEffect, useState } from 'react';
import { checkUserRole } from './services/checkUserRole';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

interface Book {
  id: number;
  title: string;
  author: string;
  isAvailable: boolean;
}

const LibrarianDashboard: React.FC = () => {
  const [isRoleChecked, setIsRoleChecked] = useState(false);
  const [roleValid, setRoleValid] = useState(true);
  const [books, setBooks] = useState<Book[]>([]);
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();
  
  useEffect(() => {
    const fetchRole = async () => {
      const roleValid = await checkUserRole('User');
      setRoleValid(roleValid);
      setIsRoleChecked(true);
    };

    fetchRole();
  }, []);

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        const response = await fetch("/api/Books/UserBooks");
        if (!response.ok) {
          throw new Error("Failed to fetch books");
        }
        const data = await response.json();
        setBooks(data);
      } catch (error) {
        console.error("Error fetching books:", error);
      }
    };

    fetchBooks();
  }, []);

  const handleLogout = async () => {
      try {
        const response = await fetch('/api/Account/logout', {
          method: 'POST',
        });
  
        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Logout failed');
        };
        navigate('/Login');
      }
      catch (error: any) {
        setErrorMessage(error.message || 'Logout failed. Please try again.');
      };
  }

  if (!isRoleChecked) {
    return <div>Loading...</div>;
  }

  if (!roleValid) {
    navigate('/unauthorized');
  }

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
      <h2>User Dashboard</h2>
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
      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Title</th>
            <th>Author</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {books.map((book) => (
            <tr key={book.id}>
              <td>{book.title}</td>
              <td>{book.author}</td>
              <td>{book.isAvailable ? "Reservable" : "Not Available"}</td>
              <td>
                <a href={`/Details/${book.id}`} className="btn btn-secondary btn-sm">Details</a> |{" "}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default LibrarianDashboard;

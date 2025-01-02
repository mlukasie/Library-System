import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

interface Book {
  id: number;
  title: string;
  author: string;
  isVisible: boolean;
}

const LibrarianDashboard: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [books, setBooks] = useState<Book[]>([]);
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();
  

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        const response = await fetch("/api/Books/LibrarianBooks");
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
      }
      
  }

  if (loading)
  {
    return  <div>Loading...</div>;
  }

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
      <h2>Librarian Dashboard</h2>
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
      <div className="mb-3">
        <button className="btn btn-primary me-2" onClick={() => navigate("/Create-book")}>
          Add New Book
        </button>
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
              <td>{book.isVisible ? "Visible to readers" : "Deleted"}</td>
              <td>
                <a href={`/Edit/${book.id}`} className="btn btn-secondary btn-sm">Edit</a> |{" "}
                <a href={`/Details/${book.id}`} className="btn btn-secondary btn-sm">Details</a> |{" "}
                <a href={`/Delete/${book.id}`} className="btn btn-danger btn-sm">Delete</a>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default LibrarianDashboard;

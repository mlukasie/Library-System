import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

interface Book {
  title: string;
  author: string;
  publisher: string;
  releaseDate: string;
}

const DetailsBook: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [book, setBook] = useState<Book | null>(null);
  const [loading, setLoading] = useState(true);
  const [userRole, setUserRole] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchBookDetails = async () => {
      try {
        // Fetch book details
        const bookResponse = await fetch(`/api/Books/${id}`);
        if (bookResponse.status === 401 || bookResponse.status === 403) {
          navigate('/unauthorized');
          return;
        }
        if (!bookResponse.ok) {
          throw new Error('Failed to fetch book details');
        }
        const bookData = await bookResponse.json();
        setBook(bookData);

        // Fetch user role
        const roleResponse = await fetch('/api/Account/role', {
          method: 'GET',
          credentials: 'include',
        });
        if (!roleResponse.ok) {
          throw new Error('Failed to fetch user role');
        }
        const roleData = await roleResponse.json();
        setUserRole(roleData.role);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchBookDetails();
  }, [id, navigate]);

  const handleBackToDashboard = () => {
    if (userRole === 'Librarian') {
      navigate('/Librarian-dashboard');
    } else if (userRole === 'User') {
      navigate('/User-dashboard');
    } else {
      throw new Error('Unknown role');
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!book) {
    return <div>Book not found.</div>;
  }

  return (
    <div className="container mt-4">
      <h2 className="mb-4">Book Details</h2>

      <div className="mb-3">
        <strong>Title:</strong> {book.title}
      </div>
      <div className="mb-3">
        <strong>Author:</strong> {book.author}
      </div>
      <div className="mb-3">
        <strong>Publisher:</strong> {book.publisher}
      </div>
      <div className="mb-3">
        <strong>Release Date:</strong> {new Date(book.releaseDate).toLocaleDateString()}
      </div>

      <div className="mt-4">
        {userRole === 'Librarian' && (
          <button className="btn btn-primary" onClick={() => navigate(`/Edit/${id}`)}>
            Edit
          </button>
        )}
        <button className="btn btn-secondary ms-2" onClick={handleBackToDashboard}>
          Back to Dashboard
        </button>
      </div>
    </div>
  );
};

export default DetailsBook;

import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

interface Book {
  title: string;
  author: string;
  publisher: string;
  releaseDate: string;
}

const DeleteBook: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [book, setBook] = useState<Book | null>(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();


  useEffect(() => {
    const fetchBookDetails = async () => {
      try {
        const response = await fetch(`/api/Books/${id}`);
        if (response.status === 401 || response.status === 403) {
          navigate('/unauthorized');
          return;
        }
        if (!response.ok) {
          throw new Error('Failed to fetch book details');
        }
        const data = await response.json();
        setBook(data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchBookDetails();
  }, [id, navigate]);


  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this book?')) {
        try {
        const response = await fetch(`/api/Books/${id}`, {
            method: 'DELETE',
        });
        if (response.status === 401 || response.status === 403) {
          navigate('/unauthorized');
          return;
        }
        if (!response.ok) {
            alert('Couldn`t delete book')
            throw new Error('Failed to delete book');
        }
        alert('Book deleted successfully!');
        navigate('/Librarian-Dashboard');
        } catch (error) {
        console.error('Error deleting book:', error);
        }
    }}

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!book) {
    return <div>Book not found.</div>;
  }

  return (
    <div className="container mt-4">
      <h2 className="mb-4">Delete Book</h2>

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
        <button className="btn btn-secondary" onClick={() => navigate("/Librarian-Dashboard")}>
          Back to Dashboard
        </button>
        <button className="btn btn-danger" onClick={() => handleDelete()}>
          Delete
        </button>
      </div>
    </div>
  );
}

export default DeleteBook;

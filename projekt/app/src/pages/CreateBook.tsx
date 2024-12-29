import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { checkUserRole } from './services/checkUserRole';
import 'bootstrap/dist/css/bootstrap.min.css';

const CreateBook: React.FC = () => {
  const [book, setBook] = useState({
      title: "",
      author: "",
      publisher: "",
      releaseDate: "",
    });
  const navigate = useNavigate();

  const [isRoleChecked, setIsRoleChecked] = useState(false);
  const [roleValid, setRoleValid] = useState(true);
  useEffect(() => {
      const fetchRole = async () => {
        const roleValid = await checkUserRole('Librarian');
        setRoleValid(roleValid);
        setIsRoleChecked(true);
      };
  
      fetchRole();
    }, []);

  if (!isRoleChecked) {
    return <div className="text-center mt-5"><strong>Loading...</strong></div>;
  }

  if (!roleValid) {
    navigate('/unauthorized');
  }

  const handleSubmit = async (e: React.FormEvent) => {
      e.preventDefault();
  
      try {
        const response = await fetch(`/api/Books/`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(book), 
        });
  
        if (!response.ok) {
          throw new Error("Failed to update book");
        }
        navigate("/librarian-dashboard");
      } catch (error) {
        console.error(error);
      }
    };

  return (
    <div className="container mt-5">
      <h2 className="text-center mb-4">Create Book</h2>
      <div className="row justify-content-center">
        <div className="col-md-8 col-lg-6">
          <form onSubmit={handleSubmit} className="shadow p-4 rounded bg-white">
            <div className="form-group mb-3">
              <label htmlFor="title" className="form-label">Title</label>
              <input
                id="title"
                type="text"
                className="form-control"
                value={book.title}
                onChange={(e) => setBook({ ...book, title: e.target.value })}
                required
              />
            </div>
            <div className="form-group mb-3">
              <label htmlFor="author" className="form-label">Author</label>
              <input
                id="author"
                type="text"
                className="form-control"
                value={book.author}
                onChange={(e) => setBook({ ...book, author: e.target.value })}
                required
              />
            </div>
            <div className="form-group mb-3">
              <label htmlFor="publisher" className="form-label">Publisher</label>
              <input
                id="publisher"
                type="text"
                className="form-control"
                value={book.publisher}
                onChange={(e) => setBook({ ...book, publisher: e.target.value })}
                required
              />
            </div>
            <div className="form-group mb-3">
              <label htmlFor="releaseDate" className="form-label">Release Date</label>
              <input
                id="releaseDate"
                type="date"
                className="form-control"
                value={book.releaseDate}
                onChange={(e) => setBook({ ...book, releaseDate: e.target.value })}
                required
              />
            </div>
            <div className="d-flex justify-content-between">
              <button className="btn btn-primary" type="submit">Save</button>
              <button
                className="btn btn-secondary"
                type="button"
                onClick={() => navigate("/Librarian-dashboard")}
              >
                Back
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default CreateBook;

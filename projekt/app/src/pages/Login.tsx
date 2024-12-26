import React, { useState } from 'react';
import "./Style.css"
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const Login: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  const handleLogin = async (event: React.FormEvent) => {
    event.preventDefault();
    try {
        const formData = {
            email: email,
            password: password,
        };
      const response = await fetch('https://localhost:7183/api/User/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Login failed');
      }
      const data = await response.json();
      navigate('/Books')
    } catch (error: any) {
      setErrorMessage(error.message || 'Login failed. Please try again.');
    }
  };

  const handleSignUp = () => {
    navigate('/signup'); // Navigate to the signup page on click
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
          <div className="col-md-6">
              <div className="card shadow-sm">
                  <div className="card-body">
                      <h4 className="card-title text-center mb-4">Login</h4>
                        {errorMessage && (
                          <div className="alert alert-danger" role="alert">
                            {errorMessage}
                          </div>
                        )}
                        <form onSubmit={handleLogin}>
                          <div className="mb-3">
                            <label htmlFor="email" className="form-label">
                              Email address
                            </label>
                            <input
                              type="email"
                              className="form-control"
                              id="email"
                              value={email}
                              onChange={(e) => setEmail(e.target.value)}
                              required
                            />
                          </div>
                          <div className="mb-3">
                            <label htmlFor="password" className="form-label">
                              Password
                            </label>
                            <input
                              type="password"
                              className="form-control"
                              id="password"
                              value={password}
                              onChange={(e) => setPassword(e.target.value)}
                              required
                            />
                          </div>
                          <button type="submit" className="btn btn-primary w-100">
                            Login
                          </button>
                        </form>
                        <p className="text-center text-muted mt-4">
                            Don’t have an account? <a className="text-decoration-none fw-bold " role="button" onClick={handleSignUp}>Sign up</a>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
  );
};


export default Login;

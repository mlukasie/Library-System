import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const Register: React.FC = () => {
  const [Email, setEmail] = useState('');
  const [FirstName, setFirstname] = useState('');
  const [LastName, setLastname] = useState('');
  const [PhoneNumber, setPhonenumber] = useState('');
  const [Password, setPassword] = useState('');
  const [phoneError, setPhoneError] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  const validatePhoneNumber = (number: string) => {
    const phoneRegex = /^[0-9]{9,15}$/;
    if (!phoneRegex.test(number)) {
      setPhoneError('Invalid phone number. Use 9 to 15 digits only.');
    } else {
      setPhoneError('');
    }
  };

  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const input = e.target.value;
    setPhonenumber(input);
    validatePhoneNumber(input);
  };

  const handleRegister = async (event: React.FormEvent) => {
    event.preventDefault();
    if (phoneError) {
      return;
    }
    try {
      const formData = {
        FirstName,
        LastName,
        PhoneNumber,
        Email,
        Password,
      };
      const response = await fetch('/api/Account/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Register failed');
      }
    } catch (error: any) {
      setErrorMessage(error.message || 'Register failed. Please try again.');
    }
    navigate('/Login');
  };

  const handleLogin = () => {
    navigate('/Login');
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-6">
          <div className="card shadow-sm">
            <div className="card-body">
              <h4 className="card-title text-center mb-4">Register</h4>
              {errorMessage && (
                <div className="alert alert-danger" role="alert">
                  {errorMessage}
                </div>
              )}
              <form onSubmit={handleRegister}>
                <div className="mb-3">
                  <label htmlFor="firstname" className="form-label">
                    First Name
                  </label>
                  <input
                    type="text"
                    className="form-control"
                    id="firstname"
                    value={FirstName}
                    onChange={(e) => setFirstname(e.target.value)}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="lastname" className="form-label">
                    Last Name
                  </label>
                  <input
                    type="text"
                    className="form-control"
                    id="lastname"
                    value={LastName}
                    onChange={(e) => setLastname(e.target.value)}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="email" className="form-label">
                    Email address
                  </label>
                  <input
                    type="email"
                    className="form-control"
                    id="email"
                    value={Email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="phonenumber" className="form-label">
                    Phone number
                  </label>
                  <input
                    type="text"
                    className="form-control"
                    id="phonenumber"
                    value={PhoneNumber}
                    onChange={handlePhoneChange}
                    required
                  />
                  {phoneError && (
                    <div className="text-danger mt-1">{phoneError}</div>
                  )}
                </div>
                <div className="mb-3">
                  <label htmlFor="password" className="form-label">
                    Password
                  </label>
                  <input
                    type="password"
                    className="form-control"
                    id="password"
                    value={Password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                </div>
                <button
                  type="submit"
                  className="btn btn-primary w-100"
                  disabled={!!phoneError} 
                >
                  Register
                </button>
              </form>
              <p className="text-center text-muted mt-4">
                You have an account?{' '}
                <a
                  className="text-decoration-none fw-bold"
                  role="button"
                  onClick={handleLogin}
                >
                  Login
                </a>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;

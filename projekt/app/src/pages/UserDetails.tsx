import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import CONFIG from '../config';

interface UserDTO {
  id: string;
  email: string;
  phoneNumber: string;
  firstName: string;
  lastName: string;
}

const UserDetails: React.FC = () => {
  const [user, setUser] = useState<UserDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [userRole, setUserRole] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUserDetails = async () => {
      try {
        const response = await fetch(`${CONFIG.API_URL}/api/Account/Details`, {
            credentials: 'include',});
        if (response.status === 401 || response.status === 403) {
          navigate('/unauthorized');
          return;
        }
        if (!response.ok) {
          throw new Error('Failed to fetch user details');
        }
        const data: UserDTO = await response.json();
        setUser(data);

        const roleResponse = await fetch(`${CONFIG.API_URL}/api/Account/role`, {
          method: 'GET',
          credentials: 'include',
        });
        if (!roleResponse.ok) {
          throw new Error('Failed to fetch user role');
        }
        const roleData = await roleResponse.json();
        setUserRole(roleData.role);
      } catch (error: any) {
        console.error('Error fetching user details:', error.message);
        setErrorMessage(error.message || 'Failed to load user details.');
      } finally {
        setLoading(false);
      }
    };

    fetchUserDetails();
  }, [navigate]);

  const handleBackToDashboard = () => {
    if (userRole === 'Librarian') {
      navigate('/Librarian-dashboard');
    } else if (userRole === 'User') {
      navigate('/User-dashboard');
    } else {
      throw new Error('Unknown role');
    }
  };

  const handleDeleteAccount = async () => {
    if (window.confirm('Are you sure you want to delete your account? This action cannot be undone.')) {
      try {
        const response = await fetch(`${CONFIG.API_URL}/api/Account/Delete`, {
          method: 'DELETE',
          credentials: 'include',
        });
        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Failed to delete account.');
        }
        alert('Account deleted successfully.');
        navigate('/Login');
      } catch (error: any) {
        console.error('Error deleting account:', error.message);
        alert(error.message || 'Failed to delete account.');
      }
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (errorMessage) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger">{errorMessage}</div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="container mt-4">
        <div className="alert alert-warning">User not found.</div>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h2>Details</h2>
      <div className="card mt-3">
        <div className="card-body">
          <h5 className="card-title">Personal Information</h5>
          <p className="card-text">
            <strong>First Name:</strong> {user.firstName}
          </p>
          <p className="card-text">
            <strong>Last Name:</strong> {user.lastName}
          </p>
          <p className="card-text">
            <strong>Email:</strong> {user.email}
          </p>
          <p className="card-text">
            <strong>Phone Number:</strong> {user.phoneNumber || 'N/A'}
          </p>
        </div>
      </div>
      <div className="mt-4">
        <button className="btn btn-primary ms-2" onClick={handleBackToDashboard}>
          Back to Dashboard
        </button>
        <button className="btn btn-danger ms-2" onClick={handleDeleteAccount}>
          Delete Account
        </button>
      </div>
    </div>
  );
};

export default UserDetails;

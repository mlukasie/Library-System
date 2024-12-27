// LibrarianDashboard.tsx
import React, { useEffect, useState } from 'react';
import { checkUserRole } from './services/checkUserRole';
import { useNavigate } from 'react-router-dom';

const LibrarianDashboard: React.FC = () => {
  const [isRoleChecked, setIsRoleChecked] = useState(false);
  const [roleValid, setRoleValid] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchRole = async () => {
      const roleValid = await checkUserRole('Librarian');
      setRoleValid(roleValid);
      setIsRoleChecked(true);
    };

    fetchRole();
  }, []);

  if (!isRoleChecked) {
    return <div>Loading...</div>;
  }

  if (!roleValid) {
    navigate('/unauthorized');
  }

  return (
    <div>
      <h1>Librarian Dashboard</h1>
    </div>
  );
};

export default LibrarianDashboard;

export const checkUserRole = async (requiredRole: string) => {
    try {
      const response = await fetch('/api/Account/role', {
        method: 'GET',
        credentials: 'include',
      });
  
      if (!response.ok) {
        throw new Error('Unauthorized');
      }
  
      const data = await response.json();
      const userRole = data.role;
  
      if (userRole !== requiredRole) {
        throw new Error('Unauthorized');
      }
  
      return true;
    } catch (error) {
      return false;
    }
  };
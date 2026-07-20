import { useState, useEffect, type ReactNode } from 'react';
import { getMeAsync } from '../apis/accountAPI';
import { AuthContext, type UserInfo } from './authContextValue';

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const restoreSession = async () => {
      try {
        const me = await getMeAsync();
        setUser({
          email: me.email,
          userId: me.userId,
          role: me.role,
          tenantId: me.tenantId,
          tenantType: me.tenantType,
          phoneNumber: me.phoneNumber,
          userName: me.userName,
          fullName: me.fullName,
        });
        setIsAuthenticated(true);
      } catch {
        setUser(null);
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };

    restoreSession();
  }, []);

  const login = (userInfo: UserInfo) => {
    setUser(userInfo);
    setIsAuthenticated(true);
  };

  const logout = () => {
    setUser(null);
    setIsAuthenticated(false);
  };

  const updateUser = (partial: Partial<UserInfo>) => {
    setUser((prev) => (prev ? { ...prev, ...partial } : prev));
  };

  const getUserRoles = (): string[] => {
    if (!user) return [];

    const roles: string[] = [];
    if (user.role) {
      roles.push(user.role);
    }

    return [...new Set(roles.map(r => r.toLowerCase()))];
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        isLoading,
        login,
        logout,
        getUserRoles,
        updateUser
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

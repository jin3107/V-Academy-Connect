import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { getMeAsync } from '../apis/accountAPI';
import { TenantType } from '../commons';

interface UserInfo {
  email: string;
  userId: string;
  role: string;
  tenantId?: string;
  tenantType?: TenantType;
  phoneNumber?: string;
  userName: string;
}

interface AuthContextType {
  user: UserInfo | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (userInfo: UserInfo) => void;
  logout: () => void;
  getUserRoles: () => string[];
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  // Khôi phục session từ httpOnly cookie khi app khởi động (vd: F5 reload)
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
        });
        setIsAuthenticated(true);
      } catch {
        // Chưa đăng nhập hoặc cookie đã hết hạn
        setUser(null);
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };

    restoreSession();
  }, []);

  const login = (userInfo: UserInfo) => {
    // Token đã được backend lưu vào httpOnly cookie, frontend không nhận
    // và không cần decode - chỉ lưu user info vào memory (React state)
    setUser(userInfo);
    setIsAuthenticated(true);
  };

  const logout = () => {
    // Xóa thông tin user khỏi memory
    setUser(null);
    setIsAuthenticated(false);

    // Cookie sẽ được xóa bởi backend khi call logout API
    // Hoặc bạn có thể xóa cookie ở client side (nếu không phải httpOnly)
  };

  const getUserRoles = (): string[] => {
    if (!user) return [];

    // Parse roles từ user info
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
        getUserRoles
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

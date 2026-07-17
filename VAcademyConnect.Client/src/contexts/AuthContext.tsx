import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';

interface UserInfo {
  email: string;
  userId: string;
  role: string;
  tenantId?: string;
  phoneNumber?: string;
  userName: string;
}

interface DecodedToken {
  role?: string;
  roles?: string[] | string;
  exp: number;
  email?: string;
  userId?: string;
  userName?: string;
  tenantId?: string;
  phoneNumber?: string;
  [key: string]: unknown;
}

interface AuthContextType {
  user: UserInfo | null;
  isAuthenticated: boolean;
  login: (token: string, userInfo: UserInfo) => void;
  logout: () => void;
  checkAuth: () => boolean;
  getUserRoles: () => string[];
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Kiểm tra auth khi component mount
  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = (): boolean => {
    try {
      // Cookies sẽ được tự động gửi với mỗi request
      // Chúng ta không cần lấy token từ localStorage nữa
      // Chỉ cần kiểm tra xem user info có tồn tại không
      
      // Nếu có user info trong state, nghĩa là đã đăng nhập
      if (user) {
        setIsAuthenticated(true);
        return true;
      }
      
      setIsAuthenticated(false);
      return false;
    } catch (err) {
      console.error('Auth check error:', err);
      setIsAuthenticated(false);
      setUser(null);
      return false;
    }
  };

  const login = (token: string, userInfo: UserInfo) => {
    try {
      // Decode token để verify và lấy thông tin
      const decoded: DecodedToken = jwtDecode(token);
      
      // Kiểm tra token hết hạn
      if (decoded.exp * 1000 < Date.now()) {
        throw new Error('Token expired');
      }

      // Lưu user info vào memory (React state) thay vì localStorage
      setUser(userInfo);
      setIsAuthenticated(true);

      // NOTE: Token sẽ được lưu trong httpOnly cookie bởi backend
      // Frontend không cần và không nên lưu token
    } catch (err) {
      console.error('Login error:', err);
      setUser(null);
      setIsAuthenticated(false);
      throw err;
    }
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
        login,
        logout,
        checkAuth,
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

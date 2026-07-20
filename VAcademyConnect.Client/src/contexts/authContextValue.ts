import { createContext } from 'react';
import { TenantType } from '../commons';

export interface UserInfo {
  email: string;
  userId: string;
  role: string;
  tenantId?: string;
  tenantType?: TenantType;
  phoneNumber?: string;
  userName: string;
  fullName?: string;
}

export interface AuthContextType {
  user: UserInfo | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (userInfo: UserInfo) => void;
  logout: () => void;
  getUserRoles: () => string[];
  updateUser: (partial: Partial<UserInfo>) => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

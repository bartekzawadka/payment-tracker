export default class TokenData {
  token = '';
  userName = '';

  isAuthenticated(): boolean {
    return !!this.token;
  }
}

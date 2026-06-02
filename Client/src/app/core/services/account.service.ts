import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Address, User } from '../../shared/models/user';
import { map, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl=environment.apiUrl;
  private http=inject(HttpClient);
  currentUser=signal<User | null>(null);

  login(values:any){
    let params=new HttpParams();
    params=params.append('useCookies',true);
    const body = {
      Email: values.email ?? values.Email,
      Password: values.password ?? values.Password,
      RememberMe: values.rememberMe ?? false
    };
    return this.http.post<User>(this.baseUrl+'login', body, { params });
  }
  register(values:any){
    return this.http.post(this.baseUrl+'account/register',values);
  }
  getUserInfo(){
    debugger;
    return this.http.get<User>(this.baseUrl+'account/user-info').pipe(
      map(user =>{
        this.currentUser.set(user);
        return user;
      }) 
    )
     
  }
  logout(){
    return this.http.post(this.baseUrl+'account/logout',{});
  }
  updateAddress(address:Address){
     return this.http.post(this.baseUrl+'account/address',address).pipe(
      tap(()=>{
        this.currentUser.update(user =>{
          if(user)user.address=address;
          return user;
        })
      })
     )
  }
  getAuthState(){
    return this.http.get<{isAuthenticated?:boolean, IsAuthenticated?:boolean}>(this.baseUrl+'account/auth-state').pipe(
      map(response => ({
        isAuthenticated: response.isAuthenticated ?? response.IsAuthenticated ?? false
      }))
    );
  }

  
}

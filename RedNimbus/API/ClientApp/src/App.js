import React from 'react';
import Routes from './components/Router'
import axios from 'axios';
import { Link, BrowserRouter as Router } from "react-router-dom";
import { Nav, Navbar, NavItem } from "react-bootstrap";
import { LinkContainer } from "react-router-bootstrap";


export default class App extends React.Component{

    constructor(props) {
        super(props);
        if (localStorage.getItem('token') === null || localStorage.getItem('token') === undefined ){
            this.state = {
                isLoggedIn: false,
                firstName: '',
                lastName: '',
                email: '',
                id: '',
                key: ''
            };
        } else {
            let token = localStorage.getItem('token');
            let self = this;
            this.state = {
                isLoggedIn: false
            }
            const options = {
                headers: { 'token': token }
            };
            axios.get('http://localhost:65001/api/user', options).then(
                (response) => { self.changeState(response) }
            );
        }
    }

    handleError(response) {
        alert("Error");
    }

    changeState = (resp) => {
        this.setState({
            isLoggedIn: true,
            firstName: resp.data.firstName,
            lastName: resp.data.lastName,
            email: resp.data.email,
            id: resp.data.id,
            key: resp.data.key
        });

        localStorage.setItem('token', this.state.key);
    } 

    signOut = () => {
        this.setState({
            isLoggedIn:false
        });

        localStorage.clear();
    }

    render()
    {
        if(this.state.isLoggedIn){
            return (
                <Router>
                <div className="App container">
                    <Navbar fluid collapseOnSelect>
                        <Navbar.Header>
                            <Navbar.Brand>
                                    <Link to="/">RedNimbus</Link>
                            </Navbar.Brand>
                            <Navbar.Toggle />
                        </Navbar.Header>
                        <Nav> 
                            <LinkContainer to="/bucket">
                                <NavItem>Bucket</NavItem>
                            </LinkContainer>
                        </Nav>
                        <Navbar.Collapse>
                            <Nav pullRight>
                                
                                <LinkContainer to="/signout">
                                    <NavItem>signOut</NavItem>
                                </LinkContainer>
                            </Nav>
                        </Navbar.Collapse>
                    </Navbar>
                    
                    <Routes changeState={this.changeState} user={this.state} signOut={this.signOut}/>
                    </div>
                </Router>
            );
        }else{
            return (
                <Router>
                <div className="App container">
                    <Navbar fluid collapseOnSelect>
                        <Navbar.Header>
                            <Navbar.Brand>
                                <LinkContainer to="/">
                                    <Link to="/">Home</Link>
                                </LinkContainer>
                            </Navbar.Brand>
                            <Navbar.Toggle />
                        </Navbar.Header>
                        <Navbar.Collapse>
                            <Nav pullRight>
                                <LinkContainer to="/login">
                                    <NavItem>Login</NavItem>
                                </LinkContainer>
                                <LinkContainer to="/register">
                                    <NavItem>Signup</NavItem>
                                </LinkContainer>
                            </Nav>
                        </Navbar.Collapse>
                    </Navbar>
                    
                    <Routes changeState={this.changeState} user={this.state} signOut={this.signOut}/>
                    
                </div>
                </Router>
            );
        }
    }
}
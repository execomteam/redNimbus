import React from 'react';
import CreateNewBucket from './CreateNewBucket'
import {Button } from "react-bootstrap";

class SideNav extends React.Component{
    render(){
        return (
            <div>
                <CreateNewBucket
                        addNewBucket = {this.props.addNewBucket}
                        show={this.props.modalShow}
                        onHide={this.props.setModalShow}
                />

                <ul className="nav flex-column">
                    <li className="nav-item">
                        <a className="nav-link active" href="/bucket">My Buckets</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="#">Shared with me</a>
                    </li>
                </ul>
            </div>
        );
    }
}

export default SideNav;

 
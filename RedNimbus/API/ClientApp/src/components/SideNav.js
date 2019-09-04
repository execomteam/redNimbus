import React from 'react';
import CreateNewBucket from './CreateNewBucket'
import DeleteBucket from './DeleteBucket'
import {Button } from "react-bootstrap";

class SideNav extends React.Component{
    /*
    <ul className="nav flex-column">
                    <li className="nav-item">
                        <a style={{hidden: true}} className="nav-link active" href="/bucket"></a>
                    </li>
                    <li className="nav-item">
                        <a style={{hidden: true}} className="nav-link" href="#"></a>
                    </li>
                </ul>
    */
    render(){
        return (
            <div>
                <div>
                    <CreateNewBucket
                        addNewBucket={this.props.addNewBucket}
                        show={this.props.createModalShow}
                        onHide={this.props.setCreateModalShow}
                    />


                </div>
                <div>
                    <DeleteBucket
                        deletingBucket={this.props.deletingBucket}
                        show={this.props.deleteModalShow}
                        onHide={this.props.setDeleteModalShow}
                    />


                </div>
            </div>
            
        );
    }
}

export default SideNav;

 
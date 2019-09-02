﻿import React from 'react';

class Folder extends React.Component {
    render() {

        return (
            <div id={this.props.id} style={{ backgroundColor: 'red' }} className="card " style={{
                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'
            }}>
                <img style={{ height: '100px', width: '100px' }} src="https://freeiconshop.com/wp-content/uploads/edd/folder-outline-filled.png" className="card-img-top" alt="..." />
                <div className="card-body">
                    <p style={{ textAlign: 'center' }} className="card-text">{this.props.name}</p>
                </div>
            </div>
        );
    }
}

export default Folder;
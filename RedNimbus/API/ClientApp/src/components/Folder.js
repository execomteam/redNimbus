import React from 'react';
import { Button } from "react-bootstrap";
import axios from 'axios';

class Folder extends React.Component {

    constructor(props) {
        super(props);
        this.deleteBucket = this.deleteBucket.bind(this);
    }

    deleteBucket(event) {
        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };

        var bucketName = this.props.name;

        axios.post("http://localhost:65001/api/bucket/deleteBucket", { Value: bucketName }, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onErrorHandler(resp) {
        alert(resp.response.data);
    }

    onSuccessHandler(resp) {
        this.props.deletingBucket(resp.data);
    }

    render() {

        return (
            <div id={this.props.id} style={{ backgroundColor: 'red' }} className="card " style={{
                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'
            }}>
                <img style={{ height: '100px', width: '100px' }} src="https://freeiconshop.com/wp-content/uploads/edd/folder-outline-filled.png" className="card-img-top" alt="..." />
                <div className="card-body">
                    <p style={{ textAlign: 'center' }} className="card-text">{this.props.name}</p>
                    <div>
                        <center>
                            <button onClick={this.deleteBucket}>Delete
                            </button>
                        </center>
                    </div>
                </div>
            </div>
        );
    }
}

export default Folder;
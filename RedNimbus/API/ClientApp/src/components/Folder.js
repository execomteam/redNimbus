import React from 'react';
import { Button } from "react-bootstrap";
import axios from 'axios';

class Folder extends React.Component {

    constructor(props) {
        super(props);
        this.deleteBucket = this.deleteBucket.bind(this);
        this.enterFolder = this.enterFolder.bind(this);
    }

    deleteBucket(event) {
        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };

        axios.post("http://localhost:65001/api/bucket/deleteBucket", { Value: this.props.name, Path: this.props.path }, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    enterFolder(event) {
        event.preventDefault();
        var bucketName = this.props.name;
        this.props.enterFolder(bucketName);
    }

    onErrorHandler(resp) {
        alert(resp.response.data.message);
    }

    onSuccessHandler(resp) {
        this.props.deletingBucket(resp.data);
    }

    render() {
        if (this.props.path == "/") {
            return (
                <div id={this.props.id} style={{ backgroundColor: 'red' }} className="card " style={{
                    width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                    alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'
                }}>
                    <img
                        style={{ height: '100px', width: '100px' }}
                        src="https://cdn0.iconfinder.com/data/icons/cleaning-and-maid-1/50/49-512.png"
                        className="card-img-top"
                        alt="..."
                        onDoubleClick={this.enterFolder}
                    />
                    <div className="card-body">
                        <p style={{ textAlign: 'center' }} className="card-text">{this.props.name}</p>
                        <div>
                            <center>
                                <Button onClick={this.deleteBucket}>
                                    Delete
                                </Button>
                            </center>
                        </div>
                    </div>
                </div>
            );
        }
            

        return (
            <div id={this.props.id} style={{ backgroundColor: 'red' }} className="card " style={{
                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'
            }}>
                <img
                    style={{ height: '100px', width: '100px' }}
                    src="https://freeiconshop.com/wp-content/uploads/edd/folder-outline-filled.png"
                    className="card-img-top"
                    alt="..."
                    onDoubleClick={this.enterFolder}
                />
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
import React from 'react';
import { Button } from "react-bootstrap";
import axios from 'axios';
import { saveAs } from 'file-saver';


class File extends React.Component {
    constructor(props) {
        super(props);
        this.deleteFile = this.deleteFile.bind(this);
        this.downloadFile = this.downloadFile.bind(this);
    }

    deleteFile(event) {
        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };

        axios.post("http://localhost:65001/api/bucket/deleteFile", { Value: this.props.name, Path: this.props.path }, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onErrorHandler(resp) {
        alert(resp.response.data);
    }

    onSuccessHandler(resp) {
        this.props.deletingFile(resp.data);
        
    }

    downloadFile(event) {
        event.preventDefault();
        const options = {
            headers: { 'token': localStorage.getItem("token") },
            responseType: 'blob'
        };

        axios.post("http://localhost:65001/api/bucket/downloadFile", { Value: this.props.name, Path: this.props.path }, options).then(
            (resp) => this.onSuccessHandlerForDownload(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }
    
    onSuccessHandlerForDownload(resp) {
        var blob = new Blob([resp.data], {type: resp.data.type});

        saveAs(blob, this.props.name);
    }

    render() {
        return (
            <div style={{ backgroundColor: 'red' }} className="card " style={{
                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'
            }}>
                <img
                    style={{ height: '100px', width: '100px' }}
                    src="https://icon-library.net/images/free-icon-file/free-icon-file-29.jpg"
                    className="card-img-top"
                    alt="..." 
                    onDoubleClick={this.downloadFile}
                />
                <div className="card-body">
                    <p style={{ textAlign: 'center' }} className="card-text">{this.props.name}</p>
                    <div>
                        <center>
                            <Button onClick={this.deleteFile}>
                                Delete
                            </Button>
                        </center>
                    </div>
                </div>
            </div>
        );
    }
}

export default File;